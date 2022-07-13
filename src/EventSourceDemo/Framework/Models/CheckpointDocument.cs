using EventStore.Client;

namespace EventSourceDemo.Framework.Models;

public class CheckpointDocument
{
    public string Key { get; set; }
    public Position Position { get; set; }    
}