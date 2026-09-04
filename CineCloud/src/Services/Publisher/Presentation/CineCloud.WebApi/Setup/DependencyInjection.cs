using BuildingBlocks.Core.Mediator;
using CineCloud.WebApi.Cache;
using CineCloud.Application;
using CineCloud.Infrastructure;
using CineCloud.Queries.Application;
using CineCloud.Queries.Infrastructure;

namespace CineCloud.WebApi.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        services.AddWriteApplication();
        services.AddWriteInfrastructure();
        services.AddQueryApplication();
        services.AddQueryInfrastructure();
        services.AddScoped<ICacheRepository, CacheRepository>();
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        return services;
    }
}