using ArticleService.Data;
using ArticleService.Workers;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ServiceDefaults;
using ServiceDefaults.Contracts;
using ServiceDefaults.Extensions;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Coordinator>();
var connectionString = builder.Configuration.GetConnectionString("RabbitMq") ??
                       throw new InvalidOperationException("Missing ConnectionStrings:RabbitMq");
builder.Services.AddEasyNetQ(connectionString);
builder.Services.AddSingleton<IMessageClient, MessageClient>();
builder.Services.AddHostedService<ArticlesWorker>();
builder.Services.AddHostedService<ArticleCacheWorker>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ArticleService";
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));
builder.Services.AddSingleton<ArticleCache>();

var app = builder.Build();
app.UseServiceDefaults();

var coordinator = app.Services.GetRequiredService<Coordinator>();
foreach (Region region in Enum.GetValues<Region>())
{
    using var db = coordinator.GetArticleDbContext(region);
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/metrics/cache", async (ArticleCache cache) =>
{
    var (hits, misses) = await cache.Stats();
    return new { hits, misses };
});

app.MapControllers();

app.Run();