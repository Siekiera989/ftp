using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Ftp.Core.Identity;

namespace Ftp.Command;

public class RmdCommand : FtpCommandBase
{
    public override string CommandName => "RMD";

    public override void Execute(FtpConnectionBase user, string path)
    {
        path = Path.Combine(user.CurrentDirectory, path);
        if (user.Filesystem.DirectoryExists(path))
        {
            if (user.Filesystem.GetFiles(path).Length > 0)
            {
                throw new FtpException(FtpStatusCode.FileActionAborted, "Directory is not empty.");
            }
            else
            {
                user.Filesystem.DeleteDirectory(path);
                user.SendResponse(FtpStatusCode.FileActionOK, "Directory deleted successfully.");
            }
        }
        else
        {
            throw new FtpException(FtpStatusCode.FileActionAborted, "Directory does not exist.");
        }
    }
}
