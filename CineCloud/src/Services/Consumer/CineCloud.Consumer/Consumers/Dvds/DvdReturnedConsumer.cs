using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Queries.Application.Features.Dvds.Commands.ReturnDvd;
using MassTransit;

namespace CineCloud.Consumer.Consumers.Dvds;

public class DvdReturnedConsumer : IConsumer<DvdReturnedEvent>
{
    private readonly IMediatorHandler _mediator;
    private readonly ILogger<DvdReturnedConsumer> _logger;

    public DvdReturnedConsumer(IMediatorHandler mediator, ILogger<DvdReturnedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DvdReturnedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentNullException(nameof(context), "Mensagem inválida");

            if (string.IsNullOrEmpty(@event.Id))
            {
                _logger.LogError("Mensagem inválida");
                throw new InvalidOperationException($"Falha ao devolver dvd {@event.Id}");
            }

            var command = new ReturnDvdCommand(@event.Id, @event.UpdatedAt);
            _logger.LogInformation($"Devolvendo dvd {@event.Id}");

            var response = await _mediator.SendCommandAndReturnBool(command, default);
            if (!response)
            {
                _logger.LogError($"Ocorreu um erro durante a devolução do dvd {@event.Id}");
                throw new InvalidOperationException($"Falha ao devolver dvd {@event.Id}");
            }

            _logger.LogInformation($"Dvd {@event.Id} devolvido com sucesso");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao consumir o DvdReturnedEvent: {ex.Message}");
            throw;
        }
    }
}