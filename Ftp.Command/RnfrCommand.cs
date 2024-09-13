using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;

namespace Ftp.Command;

public class RnfrCommand() : FtpCommandBase()
{
    public override string CommandName => "RNFR";

    public override void Execute(FtpConnectionBase user, string path)
    {
        path = Path.Combine(user.CurrentDirectory, path);
        if (user.Filesystem.FileExists(path) || user.Filesystem.DirectoryExists(path))
        {
            user.LastCommandData = path;
            user.SendResponse(FtpStatusCode.FileCommandPending, "Item selected. New item name needed.", CommandName);
        }
        else
        {
            throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, $"[{CommandName}] No item exists under that name.");
        }
    }
}
