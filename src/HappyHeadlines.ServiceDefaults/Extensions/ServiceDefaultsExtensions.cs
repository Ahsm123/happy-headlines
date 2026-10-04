using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;

namespace HappyHeadlines.ServiceDefaults.Extensions;

public static class ServiceDefaultsExtensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        var seqUrl = builder.Configuration.GetConnectionString("Seq")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:Seq");
        builder.Services.AddSerilog(config => config
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Seq(seqUrl));

        var zipkinUrl = builder.Configuration.GetConnectionString("Zipkin") 
            ?? throw new InvalidOperationException("Missing ConnectionStrings:Zipkin");
        var serviceName = builder.Environment.ApplicationName;
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing => tracing
                .AddSource(MessageClient.ActivitySourceName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddZipkinExporter(options => options.Endpoint = new Uri(zipkinUrl)));
        
        return builder;
    }
    
    public static WebApplication UseServiceDefaults(this WebApplication app)
    {
        app.UseHttpMetrics();
        app.MapMetrics();
        app.MapGet("/health", () => Results.Ok());
        app.MapGet("/whoami", () => Environment.MachineName);

        return app;
    }
}
