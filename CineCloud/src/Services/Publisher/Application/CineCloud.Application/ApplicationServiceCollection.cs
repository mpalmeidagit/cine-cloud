using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CineCloud.Application;

public static class ApplicationServiceCollection
{
    public static void AddWriteApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);
        services.AddMediatR(options => options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}