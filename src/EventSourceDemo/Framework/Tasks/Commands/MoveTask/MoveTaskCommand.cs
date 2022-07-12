using MediatR;

namespace EventSourceDemo.Framework.Tasks.Commands.MoveTask;

public record MoveTaskCommand(Guid Id, string MovedBy, BoardSections Section) : IRequest<Unit>;

public class MoveTaskCommandHandler : IRequestHandler<MoveTaskCommand, Unit>
{
    private readonly ILogger<MoveTaskCommandHandler> _logger;
    private readonly AggregateRepository _aggregateRepository;

    public MoveTaskCommandHandler(ILogger<MoveTaskCommandHandler> logger, AggregateRepository aggregateRepository)
    {
        _logger = logger;
        _aggregateRepository = aggregateRepository;
    }

    public async Task<Unit> Handle(MoveTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Move task to {Section} by {MoveBy}  for {Id}",  request.Section, request.MovedBy, request.Id);
        throw new NotImplementedException();
    }
}