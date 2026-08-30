using CineCloud.Application.Contracts;
using CineCloud.Infrastructure.Context;
using CineCloud.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CineCloud.Infrastructure.Tests;

public class InfrastructureServiceCollectionTests
{
    [Fact]
    public void AddWriteInfrastructure_ShouldRegisterWriteContextAsScoped()
    {
        var services = new ServiceCollection();

        services.AddWriteInfrastructure();

        services.Should().ContainSingle(d => d.ServiceType == typeof(CineCloudWriteContext) && d.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddWriteInfrastructure_ShouldRegisterDvdsWriteRepositoryAsScoped()
    {
        var services = new ServiceCollection();

        services.AddWriteInfrastructure();

        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(IDvdsWriteRepository) &&
            d.ImplementationType == typeof(DvdWriteRepository) &&
            d.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddWriteInfrastructure_ShouldRegisterDirectorsWriteRepositoryAsScoped()
    {
        var services = new ServiceCollection();

        services.AddWriteInfrastructure();

        services.Should().ContainSingle(d =>
            d.ServiceType == typeof(IDirectorsWriteRepository) &&
            d.ImplementationType == typeof(DirectorWriteRepository) &&
            d.Lifetime == ServiceLifetime.Scoped);
    }
}
