using CineCloud.Queries.Application.Features;
using CineCloud.Queries.Application.Features.Directors.Commands.CreateDirector;
using CineCloud.Queries.Application.Features.Directors.Queries.GetDirector;
using CineCloud.Queries.Application.Features.Dvds.Commands.CreateDvd;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CineCloud.Queries.Application.Tests;

public class ApplicationServiceCollectionTests
{
    [Fact]
    public void AddQueryApplication_ShouldRegisterMediator()
    {
        var services = new ServiceCollection();

        services.AddQueryApplication();

        services.Should().Contain(d => d.ServiceType == typeof(IMediator));
    }

    [Fact]
    public void AddQueryApplication_ShouldRegisterCommandAndQueryHandlers()
    {
        var services = new ServiceCollection();

        services.AddQueryApplication();

        services.Should().Contain(d => d.ServiceType == typeof(IRequestHandler<CreateDirectorCommand, bool>));
        services.Should().Contain(d => d.ServiceType == typeof(IRequestHandler<CreateDvdCommand, bool>));
        services.Should().Contain(d => d.ServiceType == typeof(IRequestHandler<GetDirectorQuery, GetDirectorResponse>));
    }

    [Fact]
    public void AddQueryApplication_ShouldRegisterValidatorsAsScoped()
    {
        var services = new ServiceCollection();

        services.AddQueryApplication();

        services.Should().Contain(d =>
            d.ServiceType == typeof(CreateDirectorCommandValidator) &&
            d.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddQueryApplication_ShouldReturnSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddQueryApplication();

        result.Should().BeSameAs(services);
    }
}
