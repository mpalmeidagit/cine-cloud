using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Queries.Application.Features.Directors.Commands.DeleteDirector;
using MassTransit;

namespace CineCloud.Consumer.Consumers.Directors;

public class DirectorDeletedConsumer : IConsumer<DirectorDeletedEvent>
{
    private readonly IMediatorHandler _mediator;
    private ILogger<DirectorDeletedConsumer> _logger;

    public DirectorDeletedConsumer(IMediatorHandler mediator, ILogger<DirectorDeletedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DirectorDeletedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentNullException(nameof(context), "Mensagem inválida");
            var command = new DeleteDirectorCommand(@event.Id);

            _logger.LogInformation($"Removendo diretor {@event.Id}");
            var response = await _mediator.SendCommandAndReturnBool(command, default);

            if (!response)
                throw new InvalidOperationException($"Ocorreu um erro durante o processo de remoção do diretor {@event.Id}");

            _logger.LogInformation($"Diretor {@event.Id} removido com sucesso");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao consumir o DirectorDeletedEvent: {ex.Message}");
            throw;
        }
    }

}