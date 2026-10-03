using HappyHeadlines.DraftApi.Data;
using HappyHeadlines.DraftApi.Services;
using Microsoft.EntityFrameworkCore;
using HappyHeadlines.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DraftDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("DraftDbConnection")));

builder.Services.AddScoped<IDraftService, DraftManager>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseServiceDefaults();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DraftDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
