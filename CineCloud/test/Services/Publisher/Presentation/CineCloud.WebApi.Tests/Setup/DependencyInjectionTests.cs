using BuildingBlocks.Core.Mediator;
using CineCloud.Application.Contracts;
using CineCloud.Queries.Application.Contracts;
using CineCloud.WebApi.Cache;
using CineCloud.WebApi.Setup;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CineCloud.WebApi.Tests.Setup;

public class DependencyInjectionTests
{
    [Fact]
    public void AddDependencyInjection_ShouldRegisterCacheRepositoryAsScoped()
    {
        var services = new ServiceCollection();

        services.AddDependencyInjection();

        services.Should().Contain(d =>
            d.ServiceType == typeof(ICacheRepository) &&
            d.ImplementationType == typeof(CacheRepository) &&
            d.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddDependencyInjection_ShouldRegisterMediatorHandlerAsScoped()
    {
        var services = new ServiceCollection();

        services.AddDependencyInjection();

        services.Should().Contain(d =>
            d.ServiceType == typeof(IMediatorHandler) &&
            d.ImplementationType == typeof(MediatorHandler) &&
            d.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddDependencyInjection_ShouldAggregateWriteApplicationRegistrations()
    {
        var services = new ServiceCollection();

        services.AddDependencyInjection();

        services.Should().Contain(d => d.ServiceType == typeof(IMediator));
    }

    [Fact]
    public void AddDependencyInjection_ShouldAggregateWriteInfrastructureRegistrations()
    {
        var services = new ServiceCollection();

        services.AddDependencyInjection();

        services.Should().Contain(d => d.ServiceType == typeof(IDirectorsWriteRepository));
    }

    [Fact]
    public void AddDependencyInjection_ShouldAggregateQueryInfrastructureRegistrations()
    {
        var services = new ServiceCollection();

        services.AddDependencyInjection();

        services.Should().Contain(d => d.ServiceType == typeof(IDirectorsQueryRepository));
    }
}
