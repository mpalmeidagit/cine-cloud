using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Consumer.Consumers.Directors;
using CineCloud.Queries.Application.Features.Directors.Commands.DeleteDirector;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CineCloud.Consumer.Tests.Consumers.Directors;

public class DirectorDeletedConsumerTests
{
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly DirectorDeletedConsumer _consumer;

    public DirectorDeletedConsumerTests()
    {
        _consumer = new DirectorDeletedConsumer(_mediatorMock.Object, Mock.Of<ILogger<DirectorDeletedConsumer>>());
    }

    private static Mock<ConsumeContext<DirectorDeletedEvent>> ContextFor(DirectorDeletedEvent @event)
    {
        var contextMock = new Mock<ConsumeContext<DirectorDeletedEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        return contextMock;
    }

    [Fact]
    public async Task Consume_ShouldSendDeleteDirectorCommand_WhenEventIsValid()
    {
        var @event = new DirectorDeletedEvent(Guid.NewGuid().ToString());
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(new DeleteDirectorCommand(@event.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _consumer.Consume(ContextFor(@event).Object);

        _mediatorMock.Verify(m => m.SendCommandAndReturnBool(new DeleteDirectorCommand(@event.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldThrow_WhenMediatorReturnsFalse()
    {
        var @event = new DirectorDeletedEvent(Guid.NewGuid().ToString());
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(new DeleteDirectorCommand(@event.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _consumer.Consume(ContextFor(@event).Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
