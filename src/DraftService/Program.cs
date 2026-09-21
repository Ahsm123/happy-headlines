using DraftService.Data;
using DraftService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DraftDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("DraftDbConnection")));

builder.Services.AddScoped<IDraftService, DraftManager>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DraftDbContext>();
    db.Database.Migrate();
}

app.MapGet("/health", () => Results.Ok());

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
