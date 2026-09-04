using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Infrastructure.Context;
using CineCloud.Queries.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CineCloud.Queries.Infrastructure;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddQueryInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMoviesRentalReadContext, MoviesRentalReadContext>();
        services.AddScoped<IDirectorsQueryRepository, DirectorsQueryRepository>();
        services.AddScoped<IDvdsQueryRepository, DvdsQueryRepository>();

        return services;
    }
}