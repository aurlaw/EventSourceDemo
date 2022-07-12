namespace EventSourceDemo.Framework.Exceptions;

public class TaskCompletedException : Exception
{
    public TaskCompletedException() : base("Task is completed.") { }
    
}