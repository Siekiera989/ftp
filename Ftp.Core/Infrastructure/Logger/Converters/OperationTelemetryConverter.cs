using Microsoft.ApplicationInsights.Channel;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

namespace Ftp.Core.Infrastructure.Logger.Converters;

public class OperationTelemetryConverter : TraceTelemetryConverter
{
    public const string OperationId = "Operation Id";
    public const string ParentId = "Parent Id";
    public const string OperationName = "Operation name";

    public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
    {
        foreach (var telemetry in base.Convert(logEvent, formatProvider))
        {
            if (TryGetScalarProperty(logEvent, OperationId, out var operationId))
            {
                telemetry.Context.Operation.Id = operationId.ToString();
            }

            if (TryGetScalarProperty(logEvent, ParentId, out var parentId))
            {
                telemetry.Context.Operation.ParentId = parentId.ToString();
            }

            yield return telemetry;
        }
    }

    private bool TryGetScalarProperty(LogEvent logEvent, string propertyName, out object value)
    {
        var hasScalarValue = logEvent.Properties.TryGetValue(propertyName, out var someValue)
                             && someValue is ScalarValue;

        value = hasScalarValue
            ? ((ScalarValue)someValue).Value
            : default;

        return hasScalarValue;
    }
}