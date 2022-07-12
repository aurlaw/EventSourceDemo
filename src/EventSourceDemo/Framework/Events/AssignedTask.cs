namespace EventSourceDemo.Framework.Events;

public class AssignedTask
{
    public Guid TaskId { get; set; }
    public string AssignedBy { get; set; } = null!;
    public string AssignedTo { get; set; } = null!;
}