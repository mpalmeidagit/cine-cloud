using BuildingBlocks.Core.Mediator;
using CineCloud.Consumer.Setup;
using CineCloud.Queries.Application.Contracts;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CineCloud.Consumer.Tests.Setup;

public class ConsumerConfigTests
{
    private static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EventBusSettings:HostAddress"] = "rabbitmq://localhost"
            })
            .Build();

    [Fact]
    public void AddConsumerConfig_ShouldRegisterMediatorHandlerAsScoped()
    {
        var services = new ServiceCollection();

        services.AddConsumerConfig(BuildConfiguration());

        services.Should().Contain(d =>
            d.ServiceType == typeof(IMediatorHandler) &&
            d.ImplementationType == typeof(MediatorHandler) &&
            d.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddConsumerConfig_ShouldAggregateQueryApplicationAndInfrastructureRegistrations()
    {
        var services = new ServiceCollection();

        services.AddConsumerConfig(BuildConfiguration());

        services.Should().Contain(d => d.ServiceType == typeof(IDirectorsQueryRepository));
        services.Should().Contain(d => d.ServiceType == typeof(IDvdsQueryRepository));
    }
}
