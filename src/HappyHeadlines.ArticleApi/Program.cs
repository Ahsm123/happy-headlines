using HappyHeadlines.ArticleApi.Caching;
using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.ArticleApi.Messaging;
using HappyHeadlines.ArticleApi.Services;
using HappyHeadlines.ArticleApi.Workers;
using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Events;
using HappyHeadlines.ServiceDefaults;
using HappyHeadlines.ServiceDefaults.Extensions;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Shared
builder.AddServiceDefaults();
builder.Services.AddMessaging(builder.Configuration);

// Data
builder.Services.AddSingleton<Coordinator>();

// Caching
var redisConnection = builder.Configuration.GetConnectionString("Redis")
                      ?? throw new InvalidOperationException("Missing ConnectionStrings:Redis");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
    options.InstanceName = "ArticleApi";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
builder.Services.AddSingleton<ArticleCache>();

// Services
builder.Services.AddScoped<IArticleService, ArticleService>();

// Messaging
builder.Services.AddScoped<IMessageHandler<ArticlePublishedEvent>, ArticlePublishedHandler>();
builder.Services.AddHostedService<ArticlePublishedWorker>();

// Workers
builder.Services.AddHostedService<ArticleCacheWarmupWorker>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (args.Contains("migrate"))
{
    var coordinator = app.Services.GetRequiredService<Coordinator>();
    foreach (var region in Enum.GetValues<Region>())
    {
        using var db = coordinator.GetArticleDbContext(region);
        db.Database.Migrate();
    }

    return;
}

app.UseServiceDefaults();

var articleCache = app.Services.GetRequiredService<ArticleCache>();
var hitGauge = Metrics.CreateGauge("articleHits", "cache hits");
var missGauge = Metrics.CreateGauge("articleMisses", "cache misses");
Metrics.DefaultRegistry.AddBeforeCollectCallback(async ct =>
{
    var (hits, misses) = await articleCache.Stats();
    hitGauge.Set(hits);
    missGauge.Set(misses);
});

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
