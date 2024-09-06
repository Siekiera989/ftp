using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Identity;

namespace Ftp.Command;

public class UserCommand : FtpCommandBase
{
    public override string CommandName => "USER";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        user.LastCommandData = arguments;
        user.SendResponse(331, "Username ok, need password");
    }
}
