using EasyNetQ;
using NewsletterService;
using NewsletterService.Clients;
using NewsletterService.Workers;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = "host=rabbitmq;username=kalo;password=kalo";
builder.Services.AddEasyNetQ(connectionString);
builder.Services.AddSingleton<IMessageClient, MessageClient>();
builder.Services.AddHttpClient<IArticleApiClient, ArticleApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Apis:ArticleApi"] ?? 
                                 throw new InvalidOperationException());
});
builder.Services.AddSingleton<ISubscriberClient, SubscriberClient>();
builder.Services.AddHostedService<NewsletterWorker>();



var app = builder.Build();

app.MapGet("/health", () => Results.Ok());
app.MapGet("/whoami", () => Environment.MachineName);

app.UseHttpsRedirection();
app.Run();
