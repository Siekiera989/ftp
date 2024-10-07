using System.Net;
using FluentFTP;
using FluentFTP.Helpers;

class Program
{
    static async Task Main(string[] args)
    {
        string host = "127.0.0.1";
        string username = "atrack";
        string password = "zaq12wsx";

        RunFtpTest(host, username, password, FtpDataConnectionType.AutoPassive);
        RunFtpTest(host, username, password, FtpDataConnectionType.AutoActive);
    }

    private static void RunFtpTest(string host, string username, string password, FtpDataConnectionType connectionType) 
    {
        using var client = new FtpClient(host, new NetworkCredential(username, password));
        // Set up a logger for the client
        client.Logger = new ConsoleFtpLogger();
        client.Config.DataConnectionType = connectionType;

        try
        {
            // Connect to the FTP server
            client.Connect();
            Console.WriteLine("Connected to the FTP server.");

            // List directories and files
            Console.WriteLine("\nContents of the root directory:");
            foreach (var item in client.GetListing("/"))
            {
                Console.WriteLine($"{item.FullName} - {item.Type}");
            }

            // Upload a file
            var baseLocalPath = AppDomain.CurrentDomain.BaseDirectory;

            string localPath = $"{baseLocalPath}\\Assets\\TestFile.txt";
            string remotePath = "testDirectory\\remoteFile.txt";

            Console.WriteLine("\nUploading file...");

            var uploadStatus = client.UploadFile(localPath, remotePath, createRemoteDir: true);

            Console.WriteLine($"File upload status: {uploadStatus}");

            // Download a file
            string downloadPath = $"{baseLocalPath}\\Assets\\downloadedFile.txt";

            Console.WriteLine("\nDownloading file...");
            var downloadStatus = client.DownloadFile(downloadPath, remotePath);
            Console.WriteLine($"File download status: {downloadStatus}");

            var fileExists = File.Exists(downloadPath);
            Console.WriteLine($"Downloaded file exists? {fileExists}");

            if (fileExists)
                File.Delete(downloadPath);

            // Delete a file
            Console.WriteLine("\nDeleting file...");
            client.DeleteFile(remotePath);
            Console.WriteLine("File deleted successfully!");


            // Delete directory
            Console.WriteLine("\nDeleting directory...");
            client.DeleteDirectory("testDirectory");
            Console.WriteLine("Directory deleted successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            // Close the connection
            if (client.IsConnected)
            {
                client.Disconnect();
                Console.WriteLine("Disconnected from the FTP server.");
            }
        }
    }
}

public class ConsoleFtpLogger : IFtpLogger
{
    public void Log(FtpLogEntry entry)
    {
        Console.WriteLine($"[{entry.Severity}] {entry.Message}");
    }

    public void Log(FtpTraceLevel level, string message) => Console.WriteLine($"[{level}] {message}");

    public void Log(FtpTraceLevel level, string message, object[] args) => Console.WriteLine($"[{level}] {string.Format(message, args)}");

    public void Dispose() { }
}
