using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Ftp.Core.Identity;

namespace Ftp.Command;

public class SizeCommand : FtpCommandBase
{
    public override string CommandName => "SIZE";

    public override void Execute(FtpConnectionBase user, string path)
    {
        path = Path.Combine(user.CurrentDirectory, path);
        if (user.Filesystem.FileExists(path))
        {
            var data = user.Filesystem.GetFileInfo(path).Result;
            user.SendResponse(FtpStatusCode.FileStatus, data.Length.ToString());
        }
        else
        {
            throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "File not found.");
        }
    }
}
