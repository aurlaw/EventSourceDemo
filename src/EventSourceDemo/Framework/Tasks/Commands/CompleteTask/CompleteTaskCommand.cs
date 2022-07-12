using MediatR;

namespace EventSourceDemo.Framework.Tasks.Commands.CompleteTask;

public record CompleteTaskCommand(Guid Id, string CompletedBy) : IRequest<Unit>;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, Unit>
{
    private readonly ILogger<CompleteTaskCommandHandler> _logger;
    private readonly AggregateRepository _aggregateRepository;

    public CompleteTaskCommandHandler(ILogger<CompleteTaskCommandHandler> logger, AggregateRepository aggregateRepository)
    {
        _logger = logger;
        _aggregateRepository = aggregateRepository;
    }

    public async Task<Unit> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Complete task by {CompletedBy} for {Id}", request.CompletedBy, request.Id);
        throw new NotImplementedException();
    }
}