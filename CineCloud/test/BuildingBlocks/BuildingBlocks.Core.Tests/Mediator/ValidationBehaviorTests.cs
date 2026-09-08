using BuildingBlocks.Core.Mediator;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using Xunit;

namespace BuildingBlocks.Core.Tests.Mediator;

public class ValidationBehaviorTests
{
    public record SampleRequest(string Name) : IRequest<string>;

    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoValidatorsAreRegistered()
    {
        var behavior = new ValidationBehavior<SampleRequest, string>(Array.Empty<IValidator<SampleRequest>>());
        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync("ok");

        var result = await behavior.Handle(new SampleRequest("Steven"), nextMock.Object, CancellationToken.None);

        result.Should().Be("ok");
        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationPasses()
    {
        var validatorMock = new Mock<IValidator<SampleRequest>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        var behavior = new ValidationBehavior<SampleRequest, string>(new[] { validatorMock.Object });
        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync("ok");

        var result = await behavior.Handle(new SampleRequest("Steven"), nextMock.Object, CancellationToken.None);

        result.Should().Be("ok");
        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        var failures = new List<ValidationFailure> { new("Name", "Name is required") };
        var validatorMock = new Mock<IValidator<SampleRequest>>();
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        var behavior = new ValidationBehavior<SampleRequest, string>(new[] { validatorMock.Object });
        var nextMock = new Mock<RequestHandlerDelegate<string>>();

        var act = () => behavior.Handle(new SampleRequest(""), nextMock.Object, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().ContainSingle(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required");
        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldAggregateFailures_WhenMultipleValidatorsFail()
    {
        var firstValidatorMock = new Mock<IValidator<SampleRequest>>();
        firstValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new("Name", "Name is required") }));
        var secondValidatorMock = new Mock<IValidator<SampleRequest>>();
        secondValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new("Name", "Name is too short") }));
        var behavior = new ValidationBehavior<SampleRequest, string>(new[] { firstValidatorMock.Object, secondValidatorMock.Object });
        var nextMock = new Mock<RequestHandlerDelegate<string>>();

        var act = () => behavior.Handle(new SampleRequest(""), nextMock.Object, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2);
    }
}
