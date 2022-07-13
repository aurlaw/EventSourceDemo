using System.Net;
using EventSourceDemo.Common;
using EventSourceDemo.Framework.Tasks.Commands.AssignTask;
using EventSourceDemo.Framework.Tasks.Commands.CompleteTask;
using EventSourceDemo.Framework.Tasks.Commands.CreateTask;
using EventSourceDemo.Framework.Tasks.Commands.MoveTask;
using EventSourceDemo.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventSourceDemo.EndpointDefinitions;

public class TaskEndpoints : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
    }

    
    /*
[POST] api/tasks/{id}/create
[PATCH] api/tasks/{id}/assign
[PATCH] api/tasks/{id}/move
[PATCH] api/tasks/{id}/complete 
     */
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPut("/api/tasks/{id:guid}/create", async (Guid id, [FromBody] CreateTaskDto model, ISender mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateTaskCommand(id, model.Title, model.CreatedBy);
                await mediator.Send(command, cancellationToken);
               return Results.NoContent();
            })
            .Produces((int)HttpStatusCode.NoContent)
            .WithName("CreateTask").WithTags("TasksAPI");

        app.MapMethods("/api/tasks/{id:guid}/assign", new[] { "PATCH" }, async (Guid id, [FromBody] AssignTaskDto model, ISender mediator, CancellationToken cancellationToken ) =>
            {
                var command = new AssignTaskCommand(id, model.AssignTo, model.AssignBy);
                await mediator.Send(command, cancellationToken);
                return Results.NoContent();
            })
            .Produces((int)HttpStatusCode.NoContent)
            .WithName("AssignTask").WithTags("TasksAPI");

        app.MapMethods("/api/tasks/{id:guid}/move", new[] { "PATCH" }, async (Guid id, [FromBody] MoveTaskDto model, ISender mediator, CancellationToken cancellationToken ) =>
            {
                var command = new MoveTaskCommand(id, model.MoveBy, model.Section);
                await mediator.Send(command, cancellationToken);
                return Results.NoContent();
            })
            .Produces((int)HttpStatusCode.NoContent)
            .WithName("MoveTask").WithTags("TasksAPI");
        
        app.MapMethods("/api/tasks/{id:guid}/complete", new[] { "PATCH" }, async (Guid id, [FromBody] CompleteTaskDto model, ISender mediator, CancellationToken cancellationToken ) =>
            {
                var command = new CompleteTaskCommand(id, model.CompletedBy);
                await mediator.Send(command, cancellationToken);
                return Results.NoContent();
            })
            .Produces((int)HttpStatusCode.NoContent)
            .WithName("CompleteTask").WithTags("TasksAPI");
        
        
        app.MapGet("/api/tasks/{id:guid}", (Guid id, ISender mediator,  CancellationToken cancellationToken ) => Results.NoContent())
            .Produces((int)HttpStatusCode.NoContent)
            .WithName("GetTasks").WithTags("TasksAPI");

        
    }
}
