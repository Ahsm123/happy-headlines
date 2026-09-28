using EasyNetQ;
using NewsletterService.Clients;
using NewsletterService.Workers;
using Polly;
using Polly.Extensions.Http;
using ServiceDefaults;
using ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add policies
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("RabbitMq") ??
                       throw new InvalidOperationException("Missing ConnectionStrings:RabbitMq");
builder.Services.AddEasyNetQ(connectionString);
builder.Services.AddSingleton<IMessageClient, MessageClient>();

builder.Services.AddHttpClient<IArticleApiClient, ArticleApiClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Apis:ArticleApi"] ??
                                 throw new InvalidOperationException("Missing Apis:ArticleApi")))
        .AddPolicyHandler(retryPolicy)
        .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddSingleton<ISubscriberClient, SubscriberClient>();
builder.Services.AddHostedService<NewsletterWorker>();

var app = builder.Build();
app.UseServiceDefaults();
app.Run();