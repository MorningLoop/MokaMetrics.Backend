using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MokaMetrics.API.Endpoints;
using MokaMetrics.API.Extensions;
using MokaMetrics.API.HealthChecks;
using MokaMetrics.DataAccess;
using MokaMetrics.DataAccess.Abstractions;
using MokaMetrics.DataAccess.Abstractions.Contexts;
using MokaMetrics.DataAccess.Abstractions.Influx;
using MokaMetrics.DataAccess.Abstractions.Repositories;
using MokaMetrics.DataAccess.Contexts;
using MokaMetrics.DataAccess.Repositories;
using MokaMetrics.Kafka;
using MokaMetrics.Kafka.Abstractions;
using MokaMetrics.Kafka.Consumer;
using MokaMetrics.Kafka.MessageParsers.Base;

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
builder.Services.AddSingleton<TopicProcessor>();

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
        processor.GetRequiredService<IServiceScopeFactory>()
    );
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
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.MapOpenApi();
}

//app.UseHttpsRedirection();

//configurazione websocket
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2), //la frequenza di invio di frame "ping" al client per garantire che i proxy tengano aperta la connessione. Il valore predefinito � due minuti.
};

app.UseWebSockets(webSocketOptions);


//var scopeRequiredByApi = app.Configuration["AzureAd:Scopes"] ?? "";

// add endpoint extension methods
app.MapCustomersEndPoints();
app.MapOrdersEndPoints();
app.MapTestEndpoints();

// Web sockets endpoints

app.MapWSStatusEndPoints();

app.Run();