using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Serilog;

namespace Ftp.Command;

public class RmdCommand(ILogger logger) : FtpCommandBase(logger)
{
    public override string CommandName => "RMD";

    public override void Execute(FtpConnectionBase user, string path)
    {
        path = Path.Combine(user.CurrentDirectory, path);
        if (user.Filesystem.DirectoryExists(path))
        {
            if (user.Filesystem.GetFiles(path).Length > 0)
            {
                LogError(FtpStatusCode.FileActionAborted, "Directory is not empty.");
                throw new FtpException(FtpStatusCode.FileActionAborted, "Directory is not empty.");
            }
            else
            {
                user.Filesystem.DeleteDirectory(path);
                user.SendResponse(FtpStatusCode.FileActionOK, "Directory deleted successfully.");
                LogInformation(FtpStatusCode.FileActionOK, "Directory deleted successfully.");
            }
        }
        else
        {
            LogError(FtpStatusCode.FileActionAborted, "Directory does not exist.");
            throw new FtpException(FtpStatusCode.FileActionAborted, "Directory does not exist.");
        }
    }
}
