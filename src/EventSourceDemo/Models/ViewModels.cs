using System.Text.Json.Serialization;
using EventSourceDemo.Framework;
using Newtonsoft.Json.Converters;

namespace EventSourceDemo.Models;

public class CreateTaskDto
{
    public string Title { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
}
public class AssignTaskDto
{
    public string AssignTo { get; set; } = null!;
    public string AssignBy { get; set; } = null!;
}
public class MoveTaskDto
{
    public string MoveBy { get; set; } = null!;
    
   public BoardSections Section { get; set; }
}
public class CompleteTaskDto
{
    public string CompletedBy { get; set; } = null!;
}
