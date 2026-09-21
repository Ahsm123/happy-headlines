using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ServiceDefaults;

public static class MonitorService
{
    public static readonly string ServiceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
    public static TracerProvider TracerProvider;
    public static ActivitySource ActivitySource = new ActivitySource(ServiceName);
    
    public static ILogger Log => Serilog.Log.Logger;
    static MonitorService()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // OpenTelemetry
        var zipkinEndpoint = config["Zipkin:Endpoint"] ?? "http://localhost:9411/api/v2/spans";

        TracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddZipkinExporter(o => o.Endpoint = new Uri(zipkinEndpoint))
            .AddSource(ActivitySource.Name)
            .SetResourceBuilder((ResourceBuilder.CreateDefault().AddService(ServiceName)))
            .Build();
        
        // Serilog
        var seqUrl = config["Seq:ServerUrl"] ?? "http://localhost:5341";

        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Seq(seqUrl)
            .CreateLogger();
    }
}
