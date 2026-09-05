using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Queries.Application.Features.Dvds.Commands.UpdateDvd;
using MassTransit;

namespace CineCloud.Consumer.Consumers.Dvds;

public class DvdUpdatedConsumer : IConsumer<DvdUpdatedEvent>
{
    private readonly IMediatorHandler _mediator;
    private readonly ILogger<DvdUpdatedConsumer> _logger;

    public DvdUpdatedConsumer(IMediatorHandler mediator, ILogger<DvdUpdatedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DvdUpdatedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentNullException(nameof(context), "Mensagem inválida");

            var command = new UpdateDvdCommand(
                @event.Id, 
                @event.Title, 
                @event.Genre,
                @event.Published, 
                @event.Copies, 
                @event.DirectorId, 
                @event.UpdatedAt);

            _logger.LogInformation($"Atualizando dvd {@event.Title}");
            var response = await _mediator.SendCommandAndReturnBool(command, default);

            if (!response)
            {
                _logger.LogError($"Ocorreu um erro durante a atualização do dvd {@event.Id}");
                throw new InvalidOperationException($"Falha ao atualizar o Dvd {@event.Id}");
            }

            _logger.LogInformation($"Dvd {@event.Id} atualizado com sucesso");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao consumir o DvdUpdatedEvent: {ex.Message}");
            throw;
        }
    }
}