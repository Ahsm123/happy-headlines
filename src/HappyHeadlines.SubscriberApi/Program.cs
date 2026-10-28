using HappyHeadlines.Contracts.Events;
using HappyHeadlines.ServiceDefaults;
using HappyHeadlines.ServiceDefaults.Extensions;
using HappyHeadlines.SubscriberApi.Services;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Shared
builder.AddServiceDefaults();
builder.Services.AddMessaging(builder.Configuration);

// Data

// Services
builder.Services.AddScoped<ISubscriberService, SubscriberService>();

// Messaging

// Workers


builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();

