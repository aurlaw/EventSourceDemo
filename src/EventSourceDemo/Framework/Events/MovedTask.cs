namespace EventSourceDemo.Framework.Events;

public class MovedTask
{
    public Guid TaskId { get; set; }
    public string MovedBy { get; set; } = null!;
    public BoardSections Section { get; set; }    
}