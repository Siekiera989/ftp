using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Serilog;

namespace Ftp.Command;

public class CwdCommand(ILogger logger) : FtpCommandBase(logger)
{
    public override string CommandName => "CWD";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        if (user.Filesystem.DirectoryExists(arguments))
        {
            user.CurrentDirectory = arguments;
            LogInformation(FtpStatusCode.FileActionOK, "Changed to new directory.");
            user.SendResponse(FtpStatusCode.FileActionOK, "Changed to new directory.");
        }
        else
        {
            LogError(FtpStatusCode.ActionNotTakenFileUnavailable, "Directory not found.");
            throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "Directory not found.");
        }
    }

    private static string SimplifyPath(string path)
    {
        string[] pathSections = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
        Stack<string> pathStack = new();
        for (int x = 0; x < pathSections.Length; x++)
        {
            if (pathSections[x] == "..")
            {
                pathStack.Pop();
            }
            else
            {
                pathStack.Push("/" + pathSections[x]);
            }
        }
        return pathStack.Count == 0 ? "/" : string.Concat(pathStack.Reverse());
    }
}
