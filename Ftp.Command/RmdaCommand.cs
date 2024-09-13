using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Serilog;

namespace Ftp.Command;

public class RmdaCommand(ILogger logger) : FtpCommandBase(logger)
{
    public override string CommandName => "RMDA";

    public override void Execute(FtpConnectionBase user, string path)
    {
        path = Path.Combine(user.CurrentDirectory, path);
        if (user.Filesystem.DirectoryExists(path))
        {
            user.Filesystem.DeleteDirectory(path);
            user.SendResponse(FtpStatusCode.FileActionOK, "Directory deleted successfully.");
            LogInformation(FtpStatusCode.FileActionOK, "Directory deleted successfully.");
        }
        else
        {
            LogError(FtpStatusCode.FileActionAborted, "Directory does not exist.");
            throw new FtpException(FtpStatusCode.FileActionAborted, "Directory does not exist.");
        }
    }
}
