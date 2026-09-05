using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Queries.Application.Features.Dvds.Commands.DeleteDvd;
using MassTransit;

namespace CineCloud.Consumer.Consumers.Dvds;

public class DvdDeletedConsumer : IConsumer<DvdDeletedEvent>
{
    private readonly ILogger<DvdDeletedConsumer> _logger;
    private readonly IMediatorHandler _mediator;

    public DvdDeletedConsumer(ILogger<DvdDeletedConsumer> logger, IMediatorHandler mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<DvdDeletedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentNullException(nameof(context), "Mensagem inválida");

            if (string.IsNullOrEmpty(@event.Id) || @event.DeletedAt > DateTime.Now)
            {
                _logger.LogError("Mensagem inválida");
                throw new InvalidOperationException($"Falha ao criar dvd {@event.Id}");
            }

            var command = new DeleteDvdCommand(@event.Id, @event.DeletedAt);
            _logger.LogInformation($"Excluindo dvd {@event.Id}");

            var response = await _mediator.SendCommandAndReturnBool(command, default);
            if (!response)
            {
                _logger.LogError($"Ocorreu um erro durante a exclusão do dvd {@event.Id}");
                throw new InvalidOperationException($"Falha ao excluir dvd {@event.Id}");
            }

            _logger.LogInformation($"Dvd {@event.Id} excluído com sucesso");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao consumir o DvdDeletedEvent: {ex.Message}");
            throw;
        }
    }
}