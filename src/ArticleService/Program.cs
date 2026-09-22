using ArticleService;
using ArticleService.Data;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using ServiceDefaults;
using ServiceDefaults.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Coordinator>();
var connectionString = "host=rabbitmq;username=kalo;password=kalo";
builder.Services.AddEasyNetQ(connectionString);
builder.Services.AddSingleton<IMessageClient, MessageClient>();
builder.Services.AddHostedService<ArticlesWorker>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
var app = builder.Build();

var coordinator = app.Services.GetRequiredService<Coordinator>();
foreach (Region region in Enum.GetValues<Region>())
{
    using var db = coordinator.GetArticleDbContext(region);
    db.Database.Migrate();
}

app.MapGet("/health", () => Results.Ok());

app.MapGet("/whoami", () => Environment.MachineName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
