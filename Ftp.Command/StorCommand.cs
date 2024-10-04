﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Enums;
using Ftp.Core.Exceptions;

namespace Ftp.Command;

public class StorCommand() : FtpCommandBase()
{
    public override string CommandName => "STOR";

    public override void Execute(FtpConnectionBase user, string arguments)
    {
        arguments = Path.Combine(user.CurrentDirectory, arguments);
        if (user.Filesystem.DirectoryExists(Path.GetDirectoryName(arguments)))
        {
            ExecuteAsync(user, arguments);
            user.SendResponse(FtpStatusCode.OpeningData, $"Opening {user.DataClient} mode data transfer for STOR", CommandName);
        }
        else
        {
            throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, $"[{CommandName}] Requested file action not taken.");
        }
    }

    private async Task ExecuteAsync(FtpConnectionBase user, string path)
    {
        using TcpClient client = await user.DataClient.CreateDataConnectionAsync();
        using MemoryStream stream = new();
        if (user.TransferType == TransferMode.ASCII)
        {
            await CopyStreamAsciiAsync(client.GetStream(), stream, 81920);
            await user.Filesystem.CreateFile(path, stream);
            user.SendResponse(FtpStatusCode.ClosingData, "Closing data connection, file transfer successful.", CommandName);
        }
        else if (user.TransferType == TransferMode.Binary)
        {
            await client.GetStream().CopyToAsync(stream);
            stream.Position = 0;
            await user.Filesystem.CreateFile(path, stream);
            user.SendResponse(FtpStatusCode.ClosingData, "Closing data connection, file transfer successful.", CommandName);
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
