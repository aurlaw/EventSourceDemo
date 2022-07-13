using EventSourceDemo.Framework.Events;
using EventSourceDemo.Framework.Models;
using StackExchange.Redis;

namespace EventSourceDemo.Framework.Respository;

public class TaskRepository
{

    private readonly IConnectionMultiplexer _redis;

    public TaskRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public void Save(object @event)
    {
        switch (@event)
        {
            case CreatedTask x: OnCreated(x); break;
            case AssignedTask x: OnAssigned(x); break;
            case MovedTask x: OnMoved(x); break;
            case CompletedTask x: OnCompleted(x); break;
        }
    }

    public Task<TaskDocument?> Get(Guid taskId)
    {
        var db = _redis.GetDatabase();
        
        var result = db.HashGetAll(taskId.ToString());
        if(!result.Any())
        {
            return Task.FromResult<TaskDocument?>(null);
        }
        var doc = result.ConvertFromRedis<TaskDocument>();
        return Task.FromResult<TaskDocument?>(doc);
    }

    private async void OnCreated(CreatedTask @event)
    {
        var db = _redis.GetDatabase();
        var document = new TaskDocument
        {
            Id = @event.TaskId,
            Title = @event.Title,
            Section = BoardSections.Open,
            CreatedBy = @event.CreatedBy
        };
        var redisKey = new RedisKey(@event.TaskId.ToString());
        await db.HashSetAsync(redisKey, document.ToHashEntries());
    }

    private async void OnAssigned(AssignedTask @event)
    {
        var db = _redis.GetDatabase();
        var doc = await Get(@event.TaskId);
        doc!.AssignedTo = @event.AssignedTo;
        
        var redisKey = new RedisKey(@event.TaskId.ToString());
        await db.HashSetAsync(redisKey, doc.ToHashEntries());
    }

    private async void OnMoved(MovedTask @event)
    {
        var db = _redis.GetDatabase();
        var doc = await Get(@event.TaskId);
        doc!.Section = @event.Section;
        
        var redisKey = new RedisKey(@event.TaskId.ToString());
        await db.HashSetAsync(redisKey, doc.ToHashEntries());
    }

    private async void OnCompleted(CompletedTask @event)
    {
        var db = _redis.GetDatabase();
        var doc = await Get(@event.TaskId);
        doc!.CompletedBy = @event.CompletedBy;
        
        var redisKey = new RedisKey(@event.TaskId.ToString());
        await db.HashSetAsync(redisKey, doc.ToHashEntries());
        
    }


}