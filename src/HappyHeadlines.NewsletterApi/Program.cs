using EasyNetQ;
using HappyHeadlines.NewsletterApi.Clients;
using HappyHeadlines.NewsletterApi.Workers;
using Polly;
using Polly.Extensions.Http;
using HappyHeadlines.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Shared
builder.AddServiceDefaults();
builder.Services.AddMessaging(builder.Configuration);

// Add policies
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

// Add services to the container.
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
