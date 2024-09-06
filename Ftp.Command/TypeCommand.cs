using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Enums;
using Ftp.Core.Exceptions;
using Ftp.Core.Identity;

namespace Ftp.Command;

public class TypeCommand : FtpCommandBase
{
    public override string CommandName => "TYPE";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        string[] splitArgs = arguments.Split(' ');
        string mode = splitArgs[0];
        string? format;
        if (splitArgs.Length > 1)
        {
            format = splitArgs[1] as string;
        }
        else
        {
            format = null;
        }

        switch (mode)
        {
            case "A":
                user.TransferType = TransferMode.ASCII;
                break;
            case "I":
                user.TransferType = TransferMode.Binary;
                break;
            case "E":
            case "L":
            default:
                throw new FtpException(FtpStatusCode.CommandNotImplemented, "Command not implemented for that parameter.");
        }
        if (format != null)
        {
            switch (format)
            {
                case "N":
                    break;
                case "T":
                case "C":
                default:
                    throw new FtpException(FtpStatusCode.CommandNotImplemented, "Command not implemented for that parameter.");
            }
        }
        user.SendResponse(FtpStatusCode.CommandOK, "OK");
    }
}
