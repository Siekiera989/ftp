using System.Diagnostics;
using Autofac;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Serilog.Events;
using Serilog;
using Ftp.Core.Interfaces.Settings;
using Serilog.Sinks.SystemConsole.Themes;
using Ftp.Core.Infrastructure.Logger.Converters;
using Ftp.Core.Infrastructure.Logger.Enrichers;
using Ftp.Core.Infrastructure.Logger.TelemetryInitializers;
using Ftp.Core.Constants;
using Ftp.Core.Interfaces;

namespace Ftp.Core.DependencyInjection;

public class LoggerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register((c, p) =>
        {
            var loggerSettings = c.Resolve<ILoggerSettings>();
            var serviceContext = c.Resolve<IServiceInstanceContext>();
            var telemetryClient = c.Resolve<TelemetryClient>();

            var logger = GetDefaultLoggerConfiguration(loggerSettings, serviceContext, null)
                .CreateLogger();

            return logger;
        })
            .As<ILogger>()
            .InstancePerDependency();

        builder.Register(ctx =>
        {
            var context = ctx.Resolve<IServiceInstanceContext>();
            var settings = ctx.Resolve<ILoggerSettings>();

            var instrumentationKey = settings.AppInsightsConnectionString;
            var telemetryConfiguration = CreateTelemetryConfiguration(instrumentationKey, context);
            var client = new TelemetryClient(telemetryConfiguration);

            return client;
        })
            .As<TelemetryClient>()
            .SingleInstance();
    }

    private TelemetryConfiguration CreateTelemetryConfiguration(string instrumentationKey, IServiceInstanceContext context)
    {
        var configuration = TelemetryConfiguration.CreateDefault();
        configuration.ConnectionString = instrumentationKey;
        configuration.TelemetryInitializers.Add(new ServerHostTelemetryInitializer(context));

        return configuration;
    }

    public static LoggerConfiguration GetDefaultLoggerConfiguration(ILoggerSettings loggerSettings, IServiceInstanceContext instanceContext, TelemetryClient telemetryClient = null)
    {
        var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("solution", ServerConstants.CloudRoleName)
            .Enrich.WithProperty("project", instanceContext.ProjectName)
            .Enrich.With<OperationEnricher>()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Is(loggerSettings.LogLevel);

        AddReleaseSinks(loggerConfig, loggerSettings.AppInsightsConnectionString, telemetryClient);
        AddDebugSinks(loggerConfig, instanceContext);

        return loggerConfig;
    }

    //[Conditional("RELEASE")]
    public static void AddReleaseSinks(LoggerConfiguration configuration, string appInsightsConnectionString, TelemetryClient telemetryClient)
    {
        configuration.WriteTo.ApplicationInsights(
            telemetryClient ?? new TelemetryClient(new TelemetryConfiguration() { ConnectionString = appInsightsConnectionString }),
            new OperationTelemetryConverter()
        );
    }

    //[Conditional("DEBUG")]
    public static void AddDebugSinks(LoggerConfiguration configuration, IServiceInstanceContext instanceContext)
    {
        configuration.WriteTo.Console(theme: AnsiConsoleTheme.Literate);
    }
}