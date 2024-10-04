﻿using System.Net;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;

namespace Ftp.Command;

public class CwdCommand() : FtpCommandBase()
{
    public override string CommandName => "CWD";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        if (!arguments.StartsWith("/"))
        {
            arguments = SimplifyPath(Path.Combine(user.CurrentDirectory, arguments).Replace("\\", "/"));
        }

        if (user.Filesystem.DirectoryExists(arguments))
        {
            user.CurrentDirectory = arguments;
            user.SendResponse(FtpStatusCode.FileActionOK, $"Changed to new directory.", CommandName);
        }
        else
        {
            throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, $"[{CommandName}] Directory {arguments} not found.");
        }
    }

    private static string SimplifyPath(string path)
    {
        string[] pathSections = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
        Stack<string> pathStack = new Stack<string>();
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
