using ArticleService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ArticleDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ArticleDbConnection")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok());

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<ArticleDbContext>().Database.Migrate();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
