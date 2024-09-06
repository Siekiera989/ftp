using Serilog.Events;

namespace Ftp.Core.Interfaces.Settings;

public interface ILoggerSettings
{
    string AppInsightsConnectionString { get; set; }
    LogEventLevel LogLevel { get; set; }
}
