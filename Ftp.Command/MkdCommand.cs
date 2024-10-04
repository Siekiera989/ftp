using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;

namespace Ftp.Command;

public class MkdCommand() : FtpCommandBase()
{
    public override string CommandName => "MKD";

    public override void Execute(FtpConnectionBase user, string path)
    {
        user.Filesystem.CreateDirectory(Path.Combine(user.CurrentDirectory, path));
        user.SendResponse(FtpStatusCode.PathnameCreated, "Directory created successfully.", CommandName);
    }
}
