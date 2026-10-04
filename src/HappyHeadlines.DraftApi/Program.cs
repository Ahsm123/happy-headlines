using System.Security.Principal;
using HappyHeadlines.DraftApi.Data;
using HappyHeadlines.DraftApi.Services;
using Microsoft.EntityFrameworkCore;
using HappyHeadlines.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Shared
builder.AddServiceDefaults();

// Data
builder.Services.AddDbContext<DraftDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("Database") ??
               throw new InvalidOperationException("Missing ConnectionStrings:Database")));

// Services
builder.Services.AddScoped<IDraftService, DraftService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (args.Contains("migrate"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DraftDbContext>();
    db.Database.Migrate();
    return;
}

app.UseServiceDefaults();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.Run();
