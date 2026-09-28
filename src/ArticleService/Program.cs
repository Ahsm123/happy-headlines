using ArticleService.Data;
using ArticleService.Workers;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ServiceDefaults;
using ServiceDefaults.Contracts;
using ServiceDefaults.Extensions;

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

app.MapControllers();

app.Run();