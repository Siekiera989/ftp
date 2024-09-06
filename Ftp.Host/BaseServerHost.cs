using System.Diagnostics;
using Autofac;
using Serilog;

namespace Ftp.Host;

public abstract class BaseServerHost : IDisposable
{
    /// <summary>
    /// Telemetry client for Application Insights
    /// </summary>
    //protected abstract TelemetryClient Client { get; set; }

    /// <summary>
    /// Logger
    /// </summary>
    protected abstract ILogger Logger { get; set; }

    /// <summary>
    /// Prepares default container.
    /// </summary>
    /// <returns></returns>
    public abstract ContainerBuilder InitializeContainer();

    /// <summary>
    /// Starts server with given container.
    /// </summary>
    /// <param name="container">Dependency injection container</param>
    /// <param name="token">Cancellation token</param>
    /// <returns></returns>
    public abstract Task Run(IContainer container, CancellationToken token);

    /// <summary>
    /// Should shutdown server host
    /// </summary>
    public abstract void Dispose();

    /// <summary>
    /// Helper method to handle exception's on triggered event handlers.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="handlerType"></param>
    /// <param name="hardwareId"></param>
    public async Task TryHandle(Func<Task> action, Type handlerType, string hardwareId = null)
    {
        var activity = new Activity(handlerType.Name);
        activity.AddBaggage("HardwareId", hardwareId);
        activity.Start();

        //using (var operation = Client.StartOperation<RequestTelemetry>(activity.OperationName))
        // {
        try
        {
            //operation.Telemetry.Context.GlobalProperties.Add("HardwareId", hardwareId);

            await action();
        }
        catch (Exception ex)
        {
            if (string.IsNullOrWhiteSpace(hardwareId))
                Logger.Error(ex, $"[{{scope}}] Error occurred during processing handler", handlerType.Name);
            else
                Logger.Error(ex, $"[{{scope}}][{{HardwareId}}] Error occurred during processing handler", handlerType.Name, hardwareId);
        }

        //Client.StopOperation(operation);
        // }

        activity.Stop();
    }
}