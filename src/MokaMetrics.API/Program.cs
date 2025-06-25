using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MokaMetrics.API.Endpoints;
using MokaMetrics.API.Extensions;
using MokaMetrics.API.HealthChecks;
using MokaMetrics.DataAccess;
using MokaMetrics.DataAccess.Abstractions;
using MokaMetrics.DataAccess.Abstractions.Contexts;
using MokaMetrics.DataAccess.Abstractions.Repositories;
using MokaMetrics.DataAccess.Contexts;
using MokaMetrics.DataAccess.Repositories;
using MokaMetrics.Services;
using MokaMetrics.Services.ServicesInterfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
//builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"));
});
//DataAccess
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IIndustrialFacilityRepository, IndustrialFacilityRepository>();
builder.Services.AddScoped<ILotRepository, LotRepository>();
builder.Services.AddScoped<IMachineActivityStatusRepository, MachineActivityStatusRepository>();
builder.Services.AddScoped<IMachineRepository, MachineRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//Services
builder.Services.AddScoped<IKafkaService, KafkaService>();

// Ignores cycles in JSON serialization
builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);


builder.Services.AddInfluxDb3(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddCheck<InfluxDb3HealthCheck>("influxdb");

var app = builder.Build();

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
app.MapWSStatusEndPoints();
app.MapInfluxTestEndpoints();

app.Run();