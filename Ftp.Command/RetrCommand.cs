﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Enums;
using Ftp.Core.Exceptions;

namespace Ftp.Command;

public class RetrCommand() : FtpCommandBase()
{
    public override string CommandName => "RETR";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        arguments = Path.Combine(user.CurrentDirectory, arguments);
        if (user.Filesystem.FileExists(arguments))
        {
            ExecuteAsync(user, arguments);
            user.SendResponse(FtpStatusCode.OpeningData, $"[{CommandName}] Opening {user.DataClient} mode data transfer for RETR", CommandName);
        }
        else
        {
            throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, $"[{CommandName}] Requested file action not taken.");
        }
    }

    private async Task ExecuteAsync(FtpConnectionBase user, string path)
    {
        using TcpClient client = await user.DataClient.CreateDataConnectionAsync();
        using var stream = await user.Filesystem.GetFileStreamAsync(path);
        if (user.TransferType == TransferMode.ASCII)
        {
            await CopyStreamAsciiAsync(stream, client.GetStream(), 81920);
            user.SendResponse(FtpStatusCode.ClosingData, $"Closing data connection, file transfer successful.", CommandName);
        }
        else if (user.TransferType == TransferMode.Binary)
        {
            await stream.CopyToAsync(client.GetStream());
            user.SendResponse(FtpStatusCode.ClosingData, $"Closing data connection, file transfer successful.", CommandName);
        }
        else
        {
            throw new FtpException(FtpStatusCode.CommandNotImplemented, $"[{CommandName}] Unsupported transfer mode.");
        }
    }

    private static async Task CopyStreamAsciiAsync(Stream input, Stream output, int bufferSize)
    {
        char[] buffer = new char[bufferSize];
        int count = 0;

        using StreamReader rdr = new(input);
        using StreamWriter wtr = new(output, Encoding.ASCII);
        while ((count = rdr.Read(buffer, 0, buffer.Length)) > 0)
        {
            await wtr.WriteAsync(buffer, 0, count);
        }
    }
}
