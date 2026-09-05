using CineCloud.Consumer.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConsumerConfig(builder.Configuration);

var app = builder.Build();

app.Run();

