using Microsoft.EntityFrameworkCore;
using HappyHeadlines.ProfanityApi.Data;
using HappyHeadlines.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Shared
builder.AddServiceDefaults();

// Data
builder.Services.AddDbContext<ProfanityDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("ProfanityDbConnection")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (args.Contains("migrate"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ProfanityDbContext>();
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
