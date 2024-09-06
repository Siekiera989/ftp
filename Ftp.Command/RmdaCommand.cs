using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Ftp.Core.Identity;

namespace Ftp.Command;

public class RmdaCommand : FtpCommandBase
{
    public override string CommandName => "RMDA";

    public override void Execute(FtpConnectionBase user, string path)
    {
        path = Path.Combine(user.CurrentDirectory, path);
        if (user.Filesystem.DirectoryExists(path))
        {
            user.Filesystem.DeleteDirectory(path);
            user.SendResponse(FtpStatusCode.FileActionOK, "Directory deleted successfully.");
        }
        else
        {
            throw new FtpException(FtpStatusCode.FileActionAborted, "Directory does not exist.");
        }
    }
}
