namespace EventSourceDemo.Framework.Events;

public class CreatedTask
{
    public Guid TaskId { get; set; }
    public string CreatedBy { get; set; } = null!;
    public string Title { get; set; } = null!;
}