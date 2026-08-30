using CineCloud.Application.Contracts;
using CineCloud.Infrastructure.Context;
using CineCloud.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CineCloud.Infrastructure;

public static class InfrastructureServiceCollection
{
    public static void AddWriteInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<CineCloudWriteContext>();
        services.AddScoped<IDvdsWriteRepository, DvdWriteRepository>();
        services.AddScoped<IDirectorsWriteRepository, DirectorWriteRepository>();
    }
}