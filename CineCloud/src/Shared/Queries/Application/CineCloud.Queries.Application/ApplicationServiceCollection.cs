using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CineCloud.Queries.Application;

public static class ApplicationServiceCollection
{
    public static IServiceCollection AddQueryApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);
        services.AddMediatR(options => options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}