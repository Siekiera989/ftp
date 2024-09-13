using Ftp.Core.Enums;
using Ftp.Core.Extensions;
using Ftp.Tools.App.Host;

internal class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("FTP Server spin-up!");

        ServerMode.HostType = ServerHost.Console;

        var cts = new CancellationTokenSource();

        var host = new FtpServerHost();

        await RunServer(host, cts.Token);

        Console.WriteLine("Press Q and ENTER key to exit");

        while (!Console.ReadLine().ToUpper().Equals("Q"))
            Console.WriteLine("Press Q and ENTER key to exit");

        Console.WriteLine("Finished");
    }

    private static async Task RunServer(IServerHost host, CancellationToken token) =>
        await Task.Run(async () =>
        {
            Console.WriteLine($"Starting {host.Name}");
            try
            {
                await host.Run(CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{host.Name} - Error occurred: " + ex.Message + ex.InnerException?.Message);
            }
        }, token);
}