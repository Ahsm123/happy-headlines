using EasyNetQ;
using NewsletterService;
using NewsletterService.Workers;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = "host=rabbitmq;username=kalo;password=kalo";
builder.Services.AddEasyNetQ(connectionString);
builder.Services.AddSingleton<IMessageClient, MessageClient>();
builder.Services.AddHostedService<NewsletterWorker>();


var app = builder.Build();

app.MapGet("/health", () => Results.Ok());
app.MapGet("/whoami", () => Environment.MachineName);

app.UseHttpsRedirection();
app.Run();
