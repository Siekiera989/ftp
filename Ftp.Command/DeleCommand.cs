using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Serilog;

namespace Ftp.Command;

public class DeleCommand(ILogger logger) : FtpCommandBase(logger)
{
    public override string CommandName => "DELE";

    public override void Execute(FtpConnectionBase user, string path)
    {
        path = Path.Combine(user.CurrentDirectory, path);
        if (user.Filesystem.FileExists(path))
        {
            user.Filesystem.DeleteFile(path);
            LogInformation(FtpStatusCode.FileActionOK, "File deleted successfully.");
            user.SendResponse(FtpStatusCode.FileActionOK, "File deleted successfully.");
        }
        else
        {
            LogError(FtpStatusCode.ActionNotTakenFileUnavailable, "File does not exist.");
            throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "File does not exist.");
        }
    }
}
