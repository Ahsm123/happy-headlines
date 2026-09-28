using Microsoft.EntityFrameworkCore;
using ProfanityService.Data;
using ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ProfanityDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("ProfanityDbConnection")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseServiceDefaults();

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

app.MapControllers();

app.Run();