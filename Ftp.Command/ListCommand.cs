using System.Net;
using System.Net.Sockets;
using System.Text;
using Ftp.Command.Abstract;
using Ftp.Core.Connection;
using Ftp.Core.Exceptions;
using Ftp.Core.FileSystem;

namespace Ftp.Command;

public class ListCommand() : FtpCommandBase()
{
    public override string CommandName => "LIST";

    public override void Execute(FtpConnectionBase user, string pathname)
    {
        pathname ??= "";

        pathname = Path.Combine(user.Identity.Username, user.CurrentDirectory, pathname);
        if (user.Filesystem.DirectoryExists(pathname))
        {
            ExecuteAsync(user, pathname);
            user.SendResponse(FtpStatusCode.OpeningData, $"Opening {user.DataClient.ConnectionType} mode data transfer for LIST", CommandName);
        }
        else
        {
            throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "Requested file action not taken.");
        }
    }

    private async Task ExecuteAsync(FtpConnectionBase user, string pathname)
    {
        using (TcpClient client = await user.DataClient.CreateDataConnectionAsync())
        using (StreamWriter writer = new(client.GetStream(), Encoding.ASCII))
        {
            var directories = user.Filesystem.GetSubdirectories(pathname);
            foreach (string directory in directories)
            {
                IDirectoryEntry d = await user.Filesystem.GetDirectoryInfo(directory);

                var directoryName = GetLastFolder(d.Name);

                writer.WriteLine($"{d.Permissions} 1 FTP FTP 0 {d.LastWriteTime:MMM dd HH:mm} {directoryName}");
                writer.Flush();
            }

            foreach (string file in user.Filesystem.GetFiles(pathname))
            {
                var f = await user.Filesystem.GetFileInfo(file);

                writer.WriteLine($"{f.Permissions} 1 FTP FTP {f.Length} {f.LastWriteTime:MMM dd HH:mm} {f.Name}");
                writer.Flush();
            }
        }
        user.SendResponse(FtpStatusCode.ClosingData, $"Transfer complete.", CommandName);
    }

    private static string GetLastFolder(string fullPath) 
    {
        var slashIndex = fullPath.LastIndexOf('/');
        return slashIndex >= 0 ? fullPath.Substring(slashIndex + 1) : fullPath;
    } 
}
