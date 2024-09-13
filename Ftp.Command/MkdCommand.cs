using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Serilog;

namespace Ftp.Command;

public class MkdCommand(ILogger logger) : FtpCommandBase(logger)
{
    public override string CommandName => "MKD";

    public override void Execute(FtpConnectionBase user, string path)
    {
        user.Filesystem.CreateDirectory(Path.Combine(user.CurrentDirectory, path));
        LogInformation(FtpStatusCode.PathnameCreated, "Directory created successfully.");
        user.SendResponse(FtpStatusCode.PathnameCreated, "Directory created successfully.");
    }
}
