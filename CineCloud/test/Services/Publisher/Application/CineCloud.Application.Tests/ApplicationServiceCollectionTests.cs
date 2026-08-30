using CineCloud.Application.Features.Directors.Commands.CreateDirector;
using CineCloud.Application.Features.Directors.Commands.DeleteDirector;
using CineCloud.Application.Features.Dvds.Commands.CreateDvd;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CineCloud.Application.Tests;

public class ApplicationServiceCollectionTests
{
    [Fact]
    public void AddWriteApplication_ShouldRegisterMediator()
    {
        var services = new ServiceCollection();

        services.AddWriteApplication();

        services.Should().Contain(d => d.ServiceType == typeof(IMediator));
    }

    [Fact]
    public void AddWriteApplication_ShouldRegisterCommandHandlers()
    {
        var services = new ServiceCollection();

        services.AddWriteApplication();

        services.Should().Contain(d => d.ServiceType == typeof(IRequestHandler<CreateDirectorCommand, CreateDirectorResponse>));
        services.Should().Contain(d => d.ServiceType == typeof(IRequestHandler<DeleteDirectorCommand, bool>));
        services.Should().Contain(d => d.ServiceType == typeof(IRequestHandler<CreateDvdCommand, CreateDvdResponse>));
    }

    [Fact]
    public void AddWriteApplication_ShouldRegisterValidatorsAsScoped()
    {
        var services = new ServiceCollection();

        services.AddWriteApplication();

        services.Should().Contain(d =>
            d.ServiceType == typeof(CreateDirectorCommandValidator) &&
            d.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddWriteApplication_ShouldAllowResolvingValidator()
    {
        var services = new ServiceCollection();
        services.AddWriteApplication();
        var provider = services.BuildServiceProvider();

        var validator = provider.GetService<CreateDirectorCommandValidator>();

        validator.Should().NotBeNull();
    }
}
