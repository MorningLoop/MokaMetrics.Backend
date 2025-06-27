using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
using MokaMetrics.Kafka.Configuration;
using MokaMetrics.Kafka.Consumer;
using MokaMetrics.Kafka.MessageParsers;
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
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IIndustrialFacilityRepository, IndustrialFacilityRepository>();
builder.Services.AddScoped<ILotRepository, LotRepository>();
builder.Services.AddScoped<IMachineActivityStatusRepository, MachineActivityStatusRepository>();
builder.Services.AddScoped<IMachineRepository, MachineRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// broker message parsers
builder.Services.AddTransient<IMessageParser, CncMessageParser>();
builder.Services.AddTransient<IMessageParser, LatheMessageParser>();
builder.Services.AddTransient<IMessageParser, AssemblyMessageParser>();
builder.Services.AddTransient<IMessageParser, TestingMessageParser>();
builder.Services.AddSingleton<MessageParserFactory>();


// Kafka
builder.Services.AddSingleton<IKafkaProducer>(kafka =>
{
    return new KafkaProducer(
        new KafkaSettings()
        {
            BootstrapServers = builder.Configuration["Kafka:Host"] ?? "localhost:9092",
            GroupId = builder.Configuration["Kafka:GroupId"] ?? "mokametrics-backend-producer",
            Topics = builder.Configuration.GetSection("Kafka:Topics").Get<List<string>>() ?? new List<string>(),
            Producer = new ProducerSettings
            {
                RetryCount = int.Parse(builder.Configuration["Kafka:Producer:RetryCount"] ?? "3"),
                TimeoutMs = int.Parse(builder.Configuration["Kafka:Producer:TimeoutMs"] ?? "30000"),
                Acks = builder.Configuration["Kafka:Producer:Acks"] ?? "all"
            },
            Consumer = new ConsumerSettings
            {
                AutoOffsetReset = builder.Configuration["Kafka:Consumer:AutoOffsetReset"] ?? "earliest",
                EnableAutoCommit = bool.Parse(builder.Configuration["Kafka:Consumer:EnableAutoCommit"] ?? "false"),
                SessionTimeoutMs = int.Parse(builder.Configuration["Kafka:Consumer:SessionTimeoutMs"] ?? "30000")
            }
        },
        kafka.GetRequiredService<Microsoft.Extensions.Logging.ILogger<KafkaProducer>>()
    );
});

// Ignores cycles in JSON serialization
builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);


// InfluxDb
builder.Services.AddInfluxDb3(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddCheck<InfluxDb3HealthCheck>("influxdb");

builder.Services.AddSingleton<TopicProcessor>(processor =>
{
    return new TopicProcessor(
        processor.GetRequiredService<Microsoft.Extensions.Logging.ILogger<TopicProcessor>>(),
        processor.GetRequiredService<IKafkaProducer>(),
        processor.GetRequiredService<IInfluxDb3Service>(),
        processor.GetRequiredService<MessageParserFactory>()
    );
});
var app = builder.Build();

var serviceProvider = app.Services;
var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

// Kafka consumer
var backgroundService = new KafkaConsumerService(
    new KafkaSettings()
    {
        BootstrapServers = builder.Configuration["Kafka:Consumer:Host"] ?? "localhost:9092",
        GroupId = builder.Configuration["Kafka:Consumer:GroupId"] ?? "mokametrics-backend-consumer",
        Topics = builder.Configuration.GetSection("Kafka:Topics").Get<List<string>>() ?? new List<string>(),
        Consumer = new ConsumerSettings
        {
            AutoOffsetReset = builder.Configuration["Kafka:Consumer:AutoOffsetReset"] ?? "earliest",
            EnableAutoCommit = bool.Parse(builder.Configuration["Kafka:Consumer:EnableAutoCommit"] ?? "false"),
            SessionTimeoutMs = int.Parse(builder.Configuration["Kafka:Consumer:SessionTimeoutMs"] ?? "30000")
        }
    },
    serviceProvider.GetRequiredService<ILogger<KafkaConsumerService>>()
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
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//    app.MapOpenApi();
//}

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
//wss\
//app.MapWSStatusEndPoints();
app.MapInfluxTestEndpoints();

app.Run();