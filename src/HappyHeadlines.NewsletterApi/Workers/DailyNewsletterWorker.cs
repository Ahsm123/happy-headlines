using HappyHeadlines.NewsletterApi.Services;

namespace HappyHeadlines.NewsletterApi.Workers;

public sealed class DailyNewsletterWorker(IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<INewsletterService>();
            await service.SendDailyNewsletterAsync(stoppingToken);
        }
    }
}
