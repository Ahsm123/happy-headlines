using EasyNetQ;
using HappyHeadlines.Contracts.Events;
using HappyHeadlines.NewsletterApi.Clients;
using HappyHeadlines.NewsletterApi.Messaging;
using HappyHeadlines.NewsletterApi.Services;
using HappyHeadlines.NewsletterApi.Workers;
using HappyHeadlines.ServiceDefaults;
using HappyHeadlines.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Shared
builder.AddServiceDefaults();
builder.Services.AddMessaging(builder.Configuration);

// Clients
builder.Services.AddHttpClient<IArticleApiClient, ArticleApiClient>(client =>
        client.BaseAddress = new Uri(builder.Configuration["Apis:ArticleApi"] ??
                                     throw new InvalidOperationException("Missing Apis:ArticleApi")))
    .AddStandardResilienceHandler();

builder.Services.AddSingleton<ISubscriberClient, SubscriberClient>();

// Services
builder.Services.AddScoped<INewsletterService, NewsletterService>();

// Messaging
builder.Services.AddScoped<IMessageHandler<ArticlePublishedEvent>, ArticlePublishedHandler>();
builder.Services.AddHostedService<ArticlePublishedWorker>();

// Workers
builder.Services.AddHostedService<DailyNewsletterWorker>();

var app = builder.Build();

app.UseServiceDefaults();

app.Run();
