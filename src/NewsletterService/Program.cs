using EasyNetQ;
using NewsletterService;
using NewsletterService.Clients;
using NewsletterService.Workers;
using ServiceDefaults;
using ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("RabbitMq") ??
                       throw new InvalidOperationException("Missing ConnectionStrings:RabbitMq");
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
app.UseServiceDefaults();

app.UseHttpsRedirection();
app.Run();
