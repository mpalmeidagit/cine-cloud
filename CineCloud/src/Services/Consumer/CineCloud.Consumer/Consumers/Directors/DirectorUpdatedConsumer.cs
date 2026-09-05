using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Queries.Application.Features.Directors.Commands.UpdateDirector;
using MassTransit;

namespace CineCloud.Consumer.Consumers.Directors;

public class DirectorUpdatedConsumer : IConsumer<DirectorUpdatedEvent>
{
    private readonly IMediatorHandler _mediator;
    private readonly ILogger<DirectorUpdatedConsumer> _logger;

    public DirectorUpdatedConsumer(IMediatorHandler mediator, ILogger<DirectorUpdatedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DirectorUpdatedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentNullException(nameof(context), "Message inválida");
            var command = new UpdateDirectorCommand(
                @event.Id, 
                @event.FullName, 
                @event.UpdatedAt);

            _logger.LogInformation($"Atualizando diretor {@event.FullName}");
            var response = await _mediator.SendCommandAndReturnBool(command, default);

            if (!response)
            {
                throw new Exception($"Ocorreu um erro durante a atualização do diretor {@event.FullName}");
            }
            _logger.LogInformation($"Diretor {@event.FullName} atualizado com sucesso");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao consumir o DirectorUpdatedEvent: {ex.Message}");
            throw;
        }
    }
}