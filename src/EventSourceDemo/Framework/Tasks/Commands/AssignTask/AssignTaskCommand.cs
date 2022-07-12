using MediatR;

namespace EventSourceDemo.Framework.Tasks.Commands.AssignTask;

public record AssignTaskCommand(Guid Id, string AssignTo, string AssignedBy) : IRequest<Unit>;

public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, Unit>
{
    private readonly ILogger<AssignTaskCommandHandler> _logger;
    private readonly AggregateRepository _aggregateRepository;

    public AssignTaskCommandHandler(ILogger<AssignTaskCommandHandler> logger, AggregateRepository aggregateRepository)
    {
        _logger = logger;
        _aggregateRepository = aggregateRepository;
    }

    public async Task<Unit> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assign task to {AssignTo} for {Id}", request.AssignTo, request.Id);
        throw new NotImplementedException();
    }
}