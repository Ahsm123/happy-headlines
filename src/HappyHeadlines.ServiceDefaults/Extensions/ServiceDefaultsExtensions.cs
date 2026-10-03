using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Prometheus;

namespace HappyHeadlines.ServiceDefaults.Extensions;

public static class ServiceDefaultsExtensions
{
    public static WebApplication UseServiceDefaults(this WebApplication app)
    {
        // Every service needs to touch MonitorService to run its static ctor,
        // so it starts Zipkin tracing + Seq logging                                                                                                                                              
        _ = MonitorService.TracerProvider;

        app.UseHttpMetrics();
        app.MapMetrics();

        app.MapGet("/health", () => Results.Ok());
        app.MapGet("/whoami", () => Environment.MachineName);

        return app;
    }
}