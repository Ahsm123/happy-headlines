using HappyHeadlines.CommentApi.Caching;
using HappyHeadlines.CommentApi.Clients;
using Microsoft.EntityFrameworkCore;
using HappyHeadlines.CommentApi.Data;
using HappyHeadlines.CommentApi.Services;
using Prometheus;
using HappyHeadlines.ServiceDefaults.Extensions;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Shared
builder.AddServiceDefaults();

// Data
builder.Services.AddDbContext<CommentDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("Database") ??
               throw new InvalidOperationException("Missing ConnectionStrings:Database")));

// Caching
var redisConnection = builder.Configuration.GetConnectionString("Redis")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:Redis");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
    options.InstanceName = "CommentApi";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
builder.Services.AddScoped<CommentCache>();

// Clients
builder.Services.AddHttpClient<IProfanityClient, ProfanityClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["Apis:ProfanityApi"] ??
                                throw new InvalidOperationException("Missing Apis:ProfanityApi")))
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IArticleClient, ArticleClient>(c =>
        c.BaseAddress = new Uri(builder.Configuration["Apis:ArticleApi"] ??
                                throw new InvalidOperationException("Missing Apis:ArticleApi")))
    .AddStandardResilienceHandler();

// Services
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (args.Contains("migrate"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CommentDbContext>();
    db.Database.Migrate();
    return;
}

app.UseServiceDefaults();

var hitGauge = Metrics.CreateGauge("commentHits", "cache hits");
var missGauge = Metrics.CreateGauge("commentMisses", "cache misses");
Metrics.DefaultRegistry.AddBeforeCollectCallback(() =>
{
    using var scope = app.Services.CreateScope();
    var cache = scope.ServiceProvider.GetRequiredService<CommentCache>();
    var metrics = cache.Stats();
    hitGauge.Set(metrics.Result.Hits);
    missGauge.Set(metrics.Result.Misses);
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/metrics/cache", async (CommentCache cache) =>
{
    var (hits, misses) = await cache.Stats();
    return new { hits, misses };
});

app.MapControllers();
app.Run();
