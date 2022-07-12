namespace EventSourceDemo.Framework.Events;

public class CompletedTask
{
    public Guid TaskId { get; set; }
    public string CompletedBy { get; set; } = null!;
}