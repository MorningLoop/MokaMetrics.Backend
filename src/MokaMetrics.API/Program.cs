using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using MokaMetrics.DataAccess;
using MokaMetrics.DataAccess.Abstractions;
using MokaMetrics.DataAccess.Abstractions.Contexts;
using MokaMetrics.DataAccess.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
//builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"));
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Ignores cycles in JSON serialization
builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//var scopeRequiredByApi = app.Configuration["AzureAd:Scopes"] ?? "";

// add endpoint extension methods

app.Run();