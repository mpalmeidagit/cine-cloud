using MassTransit;
using Microsoft.Extensions.Options;
using BuildingBlocks.Core.Mediator;
using CineCloud.Queries.Application;
using CineCloud.Queries.Infrastructure;
using CineCloud.Queries.Infrastructure.Settings;
using CineCloud.Consumer.Consumers.Directors;
using CineCloud.Consumer.Consumers.Dvds;
using BuildingBlocks.Core.EventBus;


namespace CineCloud.Consumer.Setup;

public static class ConsumerConfig
{
    public static IServiceCollection AddConsumerConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);
        services.AddQueryApplication();
        services.AddQueryInfrastructure();
        services.AddMassTransit(config =>
        {
            config.AddConsumer<DirectorCreatedConsumer>();
            config.AddConsumer<DirectorUpdatedConsumer>();
            config.AddConsumer<DirectorDeletedConsumer>();
            config.AddConsumer<DvdCreatedConsumer>();
            config.AddConsumer<DvdUpdatedConsumer>();
            config.AddConsumer<DvdDeletedConsumer>();
            config.AddConsumer<DvdRentedConsumer>();
            config.AddConsumer<DvdReturnedConsumer>();
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["EventBusSettings:HostAddress"]);
                cfg.ReceiveEndpoint(EventBusConstants.CREATE_DIRECTOR_QUEUE, c =>
                {
                    c.ConfigureConsumer<DirectorCreatedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.UPDATE_DIRECTOR_QUEUE, c =>
                {
                    c.ConfigureConsumer<DirectorUpdatedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.DELETE_DIRECTOR_QUEUE, c =>
                {
                    c.ConfigureConsumer<DirectorDeletedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.CREATE_DVD_QUEUE, c =>
                {
                    c.ConfigureConsumer<DvdCreatedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.UPDATE_DVD_QUEUE, c =>
                {
                    c.ConfigureConsumer<DvdUpdatedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.DELETE_DVD_QUEUE, c =>
                {
                    c.ConfigureConsumer<DvdDeletedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.RENT_DVD_QUEUE, c =>
                {
                    c.ConfigureConsumer<DvdRentedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.RETURN_DVD_QUEUE, c =>
                {
                    c.ConfigureConsumer<DvdReturnedConsumer>(ctx);
                });
            });
        });
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        return services;
    }
}