using Serilog;
using Serilog.Extensions;

namespace MiniTwit.Api.DependencyInjection;

public static class Logging
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithRequestBody()
            .Enrich.WithRequestQuery()
            .CreateLogger();

        // Replace the default logging provider with Serilog
        builder.Host.UseSerilog();

        builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
        builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        return builder;
    }
}