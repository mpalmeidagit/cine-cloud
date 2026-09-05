using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Queries.Application.Features.Dvds.Commands.CreateDvd;
using MassTransit;

namespace CineCloud.Consumer.Consumers.Dvds;

public class DvdCreatedConsumer : IConsumer<DvdCreatedEvent>
{
    private readonly IMediatorHandler _mediator;
    private readonly ILogger<DvdCreatedConsumer> _logger;

    public DvdCreatedConsumer(IMediatorHandler mediator, ILogger<DvdCreatedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DvdCreatedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentNullException(nameof(context), "Mensagem inválida");
            var command = new CreateDvdCommand(
                @event.Id,
                @event.Title,
                @event.Genre,
                @event.Published,
                @event.Available,
                @event.Copies,
                @event.DirectorId,
                @event.CreatedAt,
                @event.UpdatedAt);

            _logger.LogInformation($"Criando dvd {command.Title}");

            var response = await _mediator.SendCommandAndReturnBool(command, default);

            if (!response)
                throw new InvalidOperationException($"Ocorreu um erro durante a criação do dvd {command.Id}");

            _logger.LogInformation($"Dvd {command.Title} criado com sucesso");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao consumir o DvdCreatedEvent: {ex.Message}");
            throw;
        }
    }
}