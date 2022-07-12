using System.Text;
using System.Text.Json;
using EventStore.Client;
//using EventStore.ClientAPI;

namespace EventSourceDemo.Framework;

public class AggregateRepository
{
    // private readonly IEventStoreConnection _eventStore;

    private readonly EventStoreClient _client;
    public AggregateRepository( EventStoreClient client)
    {
        // _eventStore = eventStore;
        _client = client;
    }

    public async Task SaveAsync<T>(T aggregate) where T : Aggregate, new()
    {
        var events = aggregate.GetChanges()
            .Select(@event => new EventData(
                Uuid.NewUuid(),
                @event.GetType().Name,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event)),
                Encoding.UTF8.GetBytes(@event.GetType().FullName!)))
            .ToList();

        if (!events.Any())
        {
            return;
        }

        var streamName = GetStreamName(aggregate, aggregate.Id);

        // _ = await _eventStore.AppendToStreamAsync(streamName, ExpectedVersion.Any, events);
        await _client.AppendToStreamAsync(streamName, StreamState.Any, events);
    }    
    /*
     *		await client.AppendToStreamAsync(
				"no-stream-stream",
				StreamState.NoStream,
				new List<EventData> {
					eventDataOne
				});
     * 
     */
    
    public async Task<T> LoadAsync<T>(Guid aggregateId, CancellationToken cancellationToken) where T : Aggregate, new()
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(aggregateId));

        var aggregate = new T();
        var streamName = GetStreamName(aggregate, aggregateId);

        var result = _client.ReadStreamAsync(
            Direction.Forwards,
            streamName,
            StreamPosition.Start,
            cancellationToken: cancellationToken);

        if (result.ReadState.Result == ReadState.StreamNotFound)
            return aggregate;

        var events = await result.ToListAsync(cancellationToken);
        if (events.Any())
        {
            
            aggregate.Load(
                events.Last().Event.EventNumber.ToInt64(),
                events.Select(@event => JsonSerializer.Deserialize(Encoding.UTF8.GetString(@event.Event.Data.ToArray()), 
                    Type.GetType(Encoding.UTF8.GetString(@event.Event.Metadata.ToArray())))
                ).ToArray());
            
        }
        //       do
  //       {
  //
  //           
  //           currentSlice = await _eventStore.ReadStreamEventsForwardAsync(
  //               streamName, 
  //               nextSliceStart, 
  //               50, false
  //           );
  //           if (currentSlice.Events.Length > 0)
  //           {
  //               aggregate.Load(
  //                   currentSlice.Events.Last().Event.EventNumber,
  //                   currentSlice.Events.Select(@event => JsonSerializer.Deserialize(Encoding.UTF8.GetString(@event.OriginalEvent.Data), 
  //                       Type.GetType(Encoding.UTF8.GetString(@event.OriginalEvent.Metadata)))
  //                   ).ToArray());
  //           }
  //           
  //           nextSliceStart = currentSlice.NextEventNumber;
  //       } while (!currentSlice.IsEndOfStream);        
  //       
        return aggregate;
    }    
    
    private string GetStreamName<T>(T type, Guid aggregateId) => $"{type.GetType().Name}-{aggregateId}";

}