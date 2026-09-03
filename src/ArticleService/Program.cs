using ArticleService.Data;
using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Coordinator>();

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
