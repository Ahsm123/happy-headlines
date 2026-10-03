using System.Runtime.CompilerServices;
using EasyNetQ;
using HappyHeadlines.ArticleApi.Caching;
using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.ArticleApi.Messaging;
using HappyHeadlines.ArticleApi.Workers;
using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.ServiceDefaults.Extensions;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add messaging
builder.Services.AddMessaging(builder.Configuration);

// Add services to the container.
builder.Services.AddSingleton<Coordinator>();
builder.Services.AddHostedService<ArticlesWorker>();
builder.Services.AddHostedService<ArticleCacheWorker>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ArticleApi";
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));
builder.Services.AddSingleton<ArticleCache>();

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

using (var scope = app.Services.CreateScope())
{
    var cache = scope.ServiceProvider.GetRequiredService<ArticleCache>();
    var hitGauge = Metrics.CreateGauge("articleHits", "cache hits");
    var missGauge = Metrics.CreateGauge("articleMisses", "cache misses");
    Metrics.DefaultRegistry.AddBeforeCollectCallback(() =>
    {
        var metrics = cache.Stats();
        hitGauge.Set(metrics.Result.Hits);
        missGauge.Set(metrics.Result.Misses);
    });

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
