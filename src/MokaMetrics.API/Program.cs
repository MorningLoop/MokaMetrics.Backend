using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MokaMetrics.API.Endpoints;
using MokaMetrics.API.Extensions;
using MokaMetrics.API.HealthChecks;
using MokaMetrics.DataAccess.Abstractions.Contexts;
using MokaMetrics.DataAccess.Abstractions.Influx;
using MokaMetrics.DataAccess.Contexts;
using MokaMetrics.Kafka;
using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Kafka.Consumer;
using MokaMetrics.Kafka.MessageParsers.Base;
using MokaMetrics.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
//builder.Services.AddAuthorization();

// Postgres Db Context
builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"));
});

// DataAccess
builder.Services.AddUnitOfWork();

// Ignores cycles in JSON serialization
builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);


// broker message parsers
builder.Services.AddMessageParsers();

// Kafka producer
builder.Services.AddSingleton<IKafkaProducer>(kafka =>
{
    return new KafkaProducer(
        new ConfigUtils(builder.Configuration).GetKafkaProducerSettings(),
        kafka.GetRequiredService<Microsoft.Extensions.Logging.ILogger<KafkaProducer>>()
    );
});

// InfluxDb
builder.Services.AddInfluxDb3(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddCheck<InfluxDb3HealthCheck>("influxdb");

builder.Services.AddTransient<TopicProcessor>(processor =>
{
    return new TopicProcessor(
        processor.GetRequiredService<Microsoft.Extensions.Logging.ILogger<TopicProcessor>>(),
        processor.GetRequiredService<IKafkaProducer>(),
        processor.GetRequiredService<IInfluxDb3Service>(),
        processor.GetRequiredService<MessageParserFactory>(),
        processor.GetRequiredService<IServiceScopeFactory>(),
        processor.GetRequiredService<IHubContext<ProductionHub>>()
    );
});

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        string[] origins = builder.Configuration["Cors:AllowedOrigin"].Split("|").ToArray<string>();

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ==== APP BUILD ====
var app = builder.Build();

var serviceProvider = app.Services;
var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

// Kafka backend (iot) consumer
var backgroundService = new KafkaConsumerService(
    serviceProvider.GetRequiredService<ILogger<KafkaConsumerService>>(),
    new KafkaConsumer(
        new ConfigUtils(builder.Configuration).GetKafkaConsumerSettings(KafkaConsumerSettingsType.Backend),
        serviceProvider.GetRequiredService<ILogger<KafkaConsumer>>(),
        serviceProvider.GetRequiredService<TopicProcessor>()
    )
);

_ = Task.Run(async () =>
{
    try
    {
        await backgroundService.StartAsync(hostApplicationLifetime.ApplicationStopping);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error starting Kafka consumer service: {ex.Message}");
    }
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    //app.MapOpenApi();
    app.UseCors("AllowReactApp");
}

if (app.Environment.IsProduction())
    app.UseHttpsRedirection();

//var scopeRequiredByApi = app.Configuration["AzureAd:Scopes"] ?? "";

// add endpoint extension methods
app.MapCustomersEndPoints();
app.MapOrdersEndPoints();
app.MapIndustrialFacilityEndpoints();
app.MapTestEndpoints();

app.MapHub<ProductionHub>("/productionHub");

app.Run();