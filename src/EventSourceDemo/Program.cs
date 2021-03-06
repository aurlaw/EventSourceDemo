using System.Text.Json.Serialization;
using EventSourceDemo.Common;
using EventSourceDemo.Framework;
using EventSourceDemo.Framework.Respository;
using EventSourceDemo.HostedServices;
using EventStore.Client;
// using EventStore.ClientAPI;
using MediatR;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Json;
using Serilog;
using StackExchange.Redis;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .ReadFrom.Configuration(ctx.Configuration));    
    
    // Event Store connection
    var settings = EventStoreClientSettings
        .Create(builder.Configuration.GetValue<string>("EventStore:ConnectionString"));
    var client = new EventStoreClient(settings);
    builder.Services.AddSingleton(client);
    
    // Redis connection
    var redisMultiplexer =
        ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>("Redis:ConnectionString"));
    builder.Services.AddSingleton<IConnectionMultiplexer>(redisMultiplexer);
    
    builder.Services.AddTransient<AggregateRepository>();
    builder.Services.AddTransient<CheckpointRepository>();
    builder.Services.AddTransient<TaskRepository>();

    builder.Services.AddHostedService<TaskHostedService>();
    builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    builder.Services.Configure<MvcJsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Event Source Demo API", Version = "v1" });    
    });

    builder.Services.AddMediatR(typeof(Program).Assembly);

    builder.Services.AddEndpointDefinitions(typeof(Program));


    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Source Demo API V1");
    });

    app.UseEndpointDefinitions();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

