using MediatR;

namespace EventSourceDemo.Framework.Tasks.Commands.CreateTask;

public record CreateTaskCommand(Guid Id, string Title, string CreatedBy) : IRequest<Unit>;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Unit>
{
    private readonly ILogger<CreateTaskCommandHandler> _logger;
    private readonly AggregateRepository _aggregateRepository;
    
    public CreateTaskCommandHandler(ILogger<CreateTaskCommandHandler> logger, AggregateRepository aggregateRepository)
    {
        _logger = logger;
        _aggregateRepository = aggregateRepository;
    }

    public async Task<Unit> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create task {Title} for {Id}", request.Title, request.Id);

        var aggregate = await _aggregateRepository.LoadAsync<Framework.Entities.Task>(request.Id, cancellationToken);
        aggregate.Create(request.Id, request.Title, request.CreatedBy);

        await _aggregateRepository.SaveAsync(aggregate);
        
        return Unit.Value;
    }
}
