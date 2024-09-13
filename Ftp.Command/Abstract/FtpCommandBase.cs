using System.Net;
using Ftp.Core.Connection;
using Serilog;

namespace Ftp.Command.Abstract;

/// <summary>
/// Represents an FTP command that a user may issue to an FTP server to perform a certain action.
/// </summary>
public abstract class FtpCommandBase
{
    public ILogger Logger { get; }

    protected FtpCommandBase(ILogger logger)
    {
        Logger = logger;
        Logger = Log.ForContext(GetType());
    }

    /// <summary>
    /// The name of the command that the user should issue.
    /// </summary>
    public abstract string CommandName { get; }

    /// <summary>
    /// This method is called when a user issues this command.
    /// </summary>
    /// <param name="connection">The user who made the request.</param>
    /// <param name="arguments">Any extra data that the user sent along with the name of the request.</param>
    public abstract void Execute(FtpConnectionBase connection, string arguments);

    protected void LogInformation(FtpStatusCode statusCode, string message)
    {
        Logger.Information($"[{statusCode}][{message}]");
    }

    protected void LogWarning(FtpStatusCode statusCode, string message)
    {
        Logger.Warning($"[{statusCode}][{message}]");
    }

    protected void LogError(FtpStatusCode statusCode, string message)
    {
        Logger.Error($"[{statusCode}][{message}]");
    }
}