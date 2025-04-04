using MiniTwit.Api.Utility;
using Serilog;
using Serilog.Extensions;

namespace MiniTwit.Api.DependencyInjection;

public static class LoggingConfiguration
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
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
        builder.Logging.AddFilter(
            "Microsoft.EntityFrameworkCore.Database.Command",
            LogLevel.Warning
        );
        return builder;
    }

    public static IApplicationBuilder ConfigureSerilog(this IApplicationBuilder app)
    {
        // Enable buffering for request bodies
        app.Use(
            async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            }
        );

        // Save response body for logging
        app.Use(
            async (context, next) =>
            {
                // Swap out the response stream with a memory stream to capture output
                var originalBodyStream = context.Response.Body;
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await next();

                // Reset the response body position to read from the beginning
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                // Copy the content of the memory stream to the original stream
                await responseBody.CopyToAsync(originalBodyStream);
            }
        );

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "Handled {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = async (diagnosticContext, httpContext) =>
            {
                // Info from user request
                diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
                diagnosticContext.Set("RequestPath", httpContext.Request.Path);
                diagnosticContext.Set("RequestQuery", httpContext.Request.QueryString.ToString());

                var requestBody = await HttpHelper.GetHttpBodyAsStringAsync(
                    httpContext.Request.Body
                );
                diagnosticContext.Set("RequestBody", requestBody);

                // Info from response
                diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);

                var responseBody = await HttpHelper.GetHttpBodyAsStringAsync(
                    httpContext.Response.Body
                );
                diagnosticContext.Set("ResponseBody", responseBody);
            };
        });

        return app;
    }
}
