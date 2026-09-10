using CommentService.Clients;
using Polly;
using Polly.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using CommentService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add policies
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

// Add services to the container.
builder.Services.AddDbContext<CommentDbContext>(options => options
        .UseNpgsql(builder.Configuration.GetConnectionString("CommentDbConnection")));

builder.Services.AddHttpClient<IProfanityClient, ProfanityClient>(c =>
        c.BaseAddress = new Uri("http://profanity-service:8080"))
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CommentDbContext>();
    db.Database.Migrate();
}

app.MapGet("/health", () => Results.Ok());

app.MapGet("/whoami", () => Environment.MachineName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
