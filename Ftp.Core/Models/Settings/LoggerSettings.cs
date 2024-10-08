﻿using Ftp.Core.Interfaces.Settings;
using Serilog.Events;

namespace Ftp.Core.Models.Settings;

public class LoggerSettings : ILoggerSettings
{
    public string AppInsightsConnectionString { get; set; }
    public LogEventLevel LogLevel { get; set; }

    public LoggerSettings()
    {
    }

    public LoggerSettings(string appInsightsConnectionString, LogEventLevel logLevel)
    {
        AppInsightsConnectionString = appInsightsConnectionString;
        LogLevel = logLevel;
    }
}