using CineCloud.Infrastructure.Context;
using CineCloud.Queries.Infrastructure.Settings;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CineCloud.WebApi.Setup;

public static class ApiConfig
{

    public static IServiceCollection AddApiConfig(this IServiceCollection services, IConfiguration configuration)
    {       
        services.AddDependencyInjection();

        services.AddDbContext<CineCloudWriteContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlConnection"), opt =>
            {
                opt.EnableRetryOnFailure();
            });
        });
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetValue<string>("CacheSettings:ConnectionString");
        });
        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["EventBusSettings:HostAddress"]);
            });
        });

        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);
        services.AddSingleton<IMongoClient>(sp => new MongoClient(sp.GetRequiredService<MongoDbSettings>().ConnectionString));

        services.AddApiVersioning();
        services.AddHealthChecks()
            .AddRedis(configuration["CacheSettings:ConnectionString"], "Cache HealthCheck", HealthStatus.Degraded)
            .AddMongoDb(
                sp => sp.GetRequiredService<IMongoClient>(),
                sp => sp.GetRequiredService<MongoDbSettings>().DatabaseName,
                name: "CineCloudDb HealthCheck",
                failureStatus: HealthStatus.Degraded)
            .AddSqlServer(configuration.GetConnectionString("SqlConnection"));

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

}