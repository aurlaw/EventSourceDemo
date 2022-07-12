namespace EventSourceDemo.Framework.Exceptions;

public class TaskAlreadyCreatedException : Exception
{
    public TaskAlreadyCreatedException() : base("Task already created.") { }    
}