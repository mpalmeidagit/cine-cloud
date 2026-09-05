using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Queries.Application.Features.Dvds.Commands.RentDvd;
using MassTransit;

namespace CineCloud.Consumer.Consumers.Dvds;

public class DvdRentedConsumer : IConsumer<DvdRentedEvent>
{
    private readonly IMediatorHandler _mediator;
    private readonly ILogger<DvdRentedConsumer> _logger;

    public DvdRentedConsumer(IMediatorHandler mediator, ILogger<DvdRentedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DvdRentedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentNullException(nameof(context), "Mensagem inválida");
            if (string.IsNullOrEmpty(@event.Id))
            {
                _logger.LogError("Mensagem inválida");
                throw new InvalidOperationException($"Falha ao alugar dvd {@event.Id}");
            }

            var command = new RentDvdCommand(@event.Id, @event.UpdatedAt);
            _logger.LogInformation($"Alugando dvd {@event.Id}");

            var response = await _mediator.SendCommandAndReturnBool(command, default);
            if (!response)
            {
                _logger.LogError($"Ocorreu um erro durante o aluguel do dvd {@event.Id}");
                throw new InvalidOperationException($"Falha ao alugar dvd {@event.Id}");
            }

            _logger.LogInformation($"Dvd {@event.Id} alugado com sucesso");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao consumir o DvdRentedEvent: {ex.Message}");
            throw;
        }
    }
}