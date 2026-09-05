using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Consumer.Consumers.Directors;
using CineCloud.Queries.Application.Features.Directors.Commands.UpdateDirector;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CineCloud.Consumer.Tests.Consumers.Directors;

public class DirectorUpdatedConsumerTests
{
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly DirectorUpdatedConsumer _consumer;

    public DirectorUpdatedConsumerTests()
    {
        _consumer = new DirectorUpdatedConsumer(_mediatorMock.Object, Mock.Of<ILogger<DirectorUpdatedConsumer>>());
    }

    private static Mock<ConsumeContext<DirectorUpdatedEvent>> ContextFor(DirectorUpdatedEvent @event)
    {
        var contextMock = new Mock<ConsumeContext<DirectorUpdatedEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        return contextMock;
    }

    [Fact]
    public async Task Consume_ShouldSendUpdateDirectorCommand_WhenEventIsValid()
    {
        var @event = new DirectorUpdatedEvent(Guid.NewGuid().ToString(), "George Lucas", DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<UpdateDirectorCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _consumer.Consume(ContextFor(@event).Object);

        _mediatorMock.Verify(m => m.SendCommandAndReturnBool(
            It.Is<UpdateDirectorCommand>(c => c.Id == @event.Id && c.FullName == @event.FullName),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldThrow_WhenMediatorReturnsFalse()
    {
        var @event = new DirectorUpdatedEvent(Guid.NewGuid().ToString(), "George Lucas", DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<UpdateDirectorCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _consumer.Consume(ContextFor(@event).Object);

        await act.Should().ThrowAsync<Exception>();
    }
}
