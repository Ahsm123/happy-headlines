using Scalar.AspNetCore;
using HappyHeadlines.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Shared
builder.AddServiceDefaults();
builder.Services.AddMessaging(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseServiceDefaults();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.Run();
