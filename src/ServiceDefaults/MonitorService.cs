using Serilog;
using ILogger = Serilog.ILogger;

namespace ServiceDefaults;

public static class MonitorService
{
    public static ILogger Log => Serilog.Log.Logger;
    static MonitorService()
    {
        // In containers this must be the compose service name; on the host it is localhost.
        var seqUrl = "http://localhost:5341";

        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Seq(seqUrl)
            .CreateLogger();
    }
}
