using EasyNetQ;
using Scalar.AspNetCore;
using ServiceDefaults;

_ = MonitorService.TracerProvider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = "host=rabbitmq;username=kalo;password=kalo";
builder.Services.AddEasyNetQ(connectionString);
builder.Services.AddSingleton<IMessageClient, MessageClient>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/health", () => Results.Ok());
app.MapGet("/whoami", () => Environment.MachineName);

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
