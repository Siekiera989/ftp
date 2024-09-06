using Ftp.Core.Constants;
using Ftp.Core.Interfaces;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Ftp.Core.Infrastructure.Logger.TelemetryInitializers;

/// <summary>
/// Telemetry Initializer for server metadata
/// </summary>
public class ServerHostTelemetryInitializer(
    IServiceInstanceContext serviceInstanceContext) : ITelemetryInitializer
{
    private readonly IServiceInstanceContext _serviceInstanceContext = serviceInstanceContext;

    public void Initialize(ITelemetry telemetry)
    {
        InitializeTelemetry(telemetry);
    }

    private void InitializeTelemetry(ITelemetry telemetry)
    {
        // Set cloud role name
        if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
        {
            telemetry.Context.Cloud.RoleName = ServerConstants.CloudRoleName;
        }

        // Set cloud role instance
        if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleInstance))
        {
            telemetry.Context.Cloud.RoleInstance = _serviceInstanceContext.Name;
        }

        // Set custom props
        if (telemetry is ISupportProperties propertySetter)
        {
            SetCustomDimensions(propertySetter);
        }
    }

    private void SetCustomDimensions(ISupportProperties telemetry)
    {
        SetCustomDimension(telemetry, "project", _serviceInstanceContext.ProjectName);
        SetCustomDimension(telemetry, "solution", ServerConstants.CloudRoleName);
    }

    private static void SetCustomDimension(ISupportProperties telemetry, string key, string value)
    {
        if (telemetry is null)
            return;

        if (string.IsNullOrEmpty(key))
            return;

        if (string.IsNullOrEmpty(value))
            return;

        if (telemetry.Properties.ContainsKey(key) is false)
        {
            telemetry.Properties.Add(key, value);
        }
        else
        {
            var existingValue = telemetry.Properties[key];
            if (string.Equals(existingValue, value, StringComparison.OrdinalIgnoreCase))
            {
            }
            else
            {
                telemetry.Properties[key] = value;
            }
        }
    }
}