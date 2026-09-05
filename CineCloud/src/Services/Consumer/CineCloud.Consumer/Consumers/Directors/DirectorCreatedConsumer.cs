using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Queries.Application.Features.Directors.Commands.CreateDirector;
using MassTransit;

namespace CineCloud.Consumer.Consumers.Directors;

public class DirectorCreatedConsumer : IConsumer<DirectorCreatedEvent>
{
    private readonly IMediatorHandler _mediator;
    private ILogger<DirectorCreatedConsumer> _logger;

    public DirectorCreatedConsumer(IMediatorHandler mediator, ILogger<DirectorCreatedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DirectorCreatedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentNullException(nameof(context));

            var command = new CreateDirectorCommand(
                @event.Id,
                @event.FullName,
                @event.CreatedAt,
                @event.UpdatedAt);

            _logger.LogInformation($"Criando diretor {command.FullName}");

            var response = await _mediator.SendCommandAndReturnBool(command, default);
            if (!response)
            {
                _logger.LogError($"Algo deu errado durante a criação do diretor {@event.Id}");
                throw new InvalidOperationException($"Falha ao criar o diretor {@event.Id}");
            }
            _logger.LogInformation($"Diretor {@event.Id} criado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro ao consumir o DirectorCreatedEvent");
            throw;
        }
    }
}