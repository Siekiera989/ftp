using System.Diagnostics;
using Ftp.Core.Infrastructure.Logger.Converters;
using Serilog.Core;
using Serilog.Events;

namespace Ftp.Core.Infrastructure.Logger.Enrichers;

public class OperationEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;
        if (activity is null)
        {
            return;
        }

        logEvent.AddPropertyIfAbsent(new LogEventProperty(OperationTelemetryConverter.OperationId, new ScalarValue(activity.TraceId)));
        logEvent.AddPropertyIfAbsent(new LogEventProperty(OperationTelemetryConverter.OperationName, new ScalarValue(activity.OperationName)));

        var hardwareId = activity.GetBaggageItem("HardwareId");
        if (hardwareId != null)
            logEvent.AddPropertyIfAbsent(new LogEventProperty("HardwareId", new ScalarValue(hardwareId)));

        if (activity.Parent == null)
        {
            return;
        }

        var parentId = $"|{activity.TraceId}.{activity.Parent.SpanId}.";
        logEvent.AddPropertyIfAbsent(new LogEventProperty(OperationTelemetryConverter.ParentId, new ScalarValue(parentId)));
    }
}