using System.Text;
using System.Text.Json;
using EventSourceDemo.Framework.Events;
using EventSourceDemo.Framework.Respository;
using EventStore.Client;

namespace EventSourceDemo.HostedServices;

public class TaskHostedService : IHostedService
{
    private readonly EventStoreClient _client;
    private readonly CheckpointRepository _checkpointRepository;
    private readonly TaskRepository _taskRepository;
    private readonly ILogger<TaskHostedService> _logger;

    public TaskHostedService(CheckpointRepository checkpointRepository, TaskRepository taskRepository, ILogger<TaskHostedService> logger, EventStoreClient client)
    {
        _checkpointRepository = checkpointRepository;
        _taskRepository = taskRepository;
        _logger = logger;
        _client = client;
    }
    private StreamSubscription _subscription;

    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var lastCheckpoint = await _checkpointRepository.GetAsync("tasks");

        var position = lastCheckpoint.HasValue ? FromAll.After(lastCheckpoint.Value) : FromAll.Start;
        
        _subscription = await _client.SubscribeToAllAsync(position, async (subscription, @event, cancellationToken) =>
            {
                if (@event.OriginalEvent.EventType.StartsWith("$"))
                    return;
                try
                {
                    var eventType = Type.GetType(Encoding.UTF8.GetString(@event.OriginalEvent.Metadata.ToArray()));
                    var eventData = JsonSerializer.Deserialize(Encoding.UTF8.GetString(@event.OriginalEvent.Data.ToArray()), 
                        eventType!);

                    if (eventType != typeof(CreatedTask) && eventType != typeof(AssignedTask) && eventType != typeof(MovedTask) && eventType != typeof(CompletedTask))
                        return;
                    if (eventData is not null)
                    {
                        _taskRepository.Save(eventData);
                    }
                    await _checkpointRepository.SaveAsync("tasks", @event.OriginalPosition.GetValueOrDefault());
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Subscription Error");
                }                
            } , 
            false, cancellationToken: cancellationToken);
        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription.Dispose();
        return Task.CompletedTask;
    }
}