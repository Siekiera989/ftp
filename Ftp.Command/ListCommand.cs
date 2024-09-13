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

        pathname = Path.Combine(user.CurrentDirectory, pathname);
        if (user.Filesystem.DirectoryExists(pathname))
        {
            ExecuteAsync(user, pathname);
            user.SendResponse(FtpStatusCode.OpeningData, $"Opening {user.DataClient} mode data transfer for LIST", CommandName);
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

                string date = d.LastWriteTime < DateTime.Now - TimeSpan.FromDays(180) ?
                    d.LastWriteTime.ToString("MMM dd  yyyy") :
                    d.LastWriteTime.ToString("MMM dd HH:mm");

                writer.WriteLine($"{d.Permissions} 1 FTP FTP 0 {date} /{directory}");
                writer.Flush();
            }

            foreach (string file in user.Filesystem.GetFiles(pathname))
            {
                var f = await user.Filesystem.GetFileInfo(file);

                string date = f.LastWriteTime < DateTime.Now - TimeSpan.FromDays(180) ?
                    f.LastWriteTime.ToString("MMMdd yyyy") :
                    f.LastWriteTime.ToString("MMMddHH:mm");

                writer.WriteLine($"{user.Filesystem.GetFilePermissions(file, user.Identity)} 1 FTP FTP {f.Length} {date} {f.Name}");
                writer.Flush();
            }
        }
        user.SendResponse(FtpStatusCode.ClosingData, $"Transfer complete.", CommandName);
    }
}
