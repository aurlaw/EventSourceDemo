namespace EventSourceDemo.Framework.Exceptions;

public class TaskNotFoundException : Exception
{
    public TaskNotFoundException() : base("Task not found.") { }
    
}