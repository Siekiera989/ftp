using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Serilog;

namespace Ftp.Command;

public class RnfrCommand(ILogger logger) : FtpCommandBase(logger)
{
    public override string CommandName => "RNFR";

    public override void Execute(FtpConnectionBase user, string path)
    {
        path = Path.Combine(user.CurrentDirectory, path);
        if (user.Filesystem.FileExists(path) || user.Filesystem.DirectoryExists(path))
        {
            user.LastCommandData = path;
            user.SendResponse(FtpStatusCode.FileCommandPending, "Item selected.  New item name needed.");
            LogInformation(FtpStatusCode.FileCommandPending, "Item selected.  New item name needed.");
        }
        else
        {
            LogError(FtpStatusCode.ActionNotTakenFileUnavailable, "No item exists under that name.");
            throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "No item exists under that name.");
        }
    }
}
