using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;

namespace Ftp.Command;

public class DeleCommand() : FtpCommandBase()
{
    public override string CommandName => "DELE";

    public override void Execute(FtpConnectionBase user, string path)
    {
        path = Path.Combine(user.CurrentDirectory, path);
        if (user.Filesystem.FileExists(path))
        {
            user.Filesystem.DeleteFile(path);
            user.SendResponse(FtpStatusCode.FileActionOK, $"File deleted successfully.", CommandName);
        }
        else
        {
            throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, $"[{CommandName}] File does not exist.");
        }
    }
}
