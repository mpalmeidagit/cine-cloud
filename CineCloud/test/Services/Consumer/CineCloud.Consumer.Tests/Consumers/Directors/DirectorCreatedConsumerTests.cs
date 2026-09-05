using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Consumer.Consumers.Directors;
using CineCloud.Queries.Application.Features.Directors.Commands.CreateDirector;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CineCloud.Consumer.Tests.Consumers.Directors;

public class DirectorCreatedConsumerTests
{
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly DirectorCreatedConsumer _consumer;

    public DirectorCreatedConsumerTests()
    {
        _consumer = new DirectorCreatedConsumer(_mediatorMock.Object, Mock.Of<ILogger<DirectorCreatedConsumer>>());
    }

    private static Mock<ConsumeContext<DirectorCreatedEvent>> ContextFor(DirectorCreatedEvent @event)
    {
        var contextMock = new Mock<ConsumeContext<DirectorCreatedEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        return contextMock;
    }

    [Fact]
    public async Task Consume_ShouldSendCreateDirectorCommand_WhenEventIsValid()
    {
        var @event = new DirectorCreatedEvent(Guid.NewGuid().ToString(), "Steven Spielberg", DateTime.Now, DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<CreateDirectorCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _consumer.Consume(ContextFor(@event).Object);

        _mediatorMock.Verify(m => m.SendCommandAndReturnBool(
            It.Is<CreateDirectorCommand>(c => c.Id == @event.Id && c.FullName == @event.FullName),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldThrow_WhenMediatorReturnsFalse()
    {
        var @event = new DirectorCreatedEvent(Guid.NewGuid().ToString(), "Steven Spielberg", DateTime.Now, DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<CreateDirectorCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _consumer.Consume(ContextFor(@event).Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
