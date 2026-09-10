using Microsoft.EntityFrameworkCore;
using ProfanityService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ProfanityDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("ProfanityDbConnection")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProfanityDbContext>();
    db.Database.Migrate();
}

app.MapGet("/health", () => Results.Ok());

app.MapGet("/whoami", () => Environment.MachineName);

app.MapControllers();

app.Run();
