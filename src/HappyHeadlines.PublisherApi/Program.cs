using EasyNetQ;
using Scalar.AspNetCore;
using HappyHeadlines.ServiceDefaults;
using HappyHeadlines.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("RabbitMq") ??
                       throw new InvalidOperationException("Missing ConnectionStrings:RabbitMq");
builder.Services.AddEasyNetQ(connectionString);
builder.Services.AddSingleton<IMessageClient, MessageClient>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseServiceDefaults();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.Run();