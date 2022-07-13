namespace EventSourceDemo.Framework.Models;

public class TaskDocument
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public string? AssignedTo { get; set; }
    public BoardSections Section { get; set; }
    public string? CompletedBy { get; set; }    
}