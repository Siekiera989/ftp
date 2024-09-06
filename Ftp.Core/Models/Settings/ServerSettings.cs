using Ftp.Core.Interfaces.Settings;

namespace Ftp.Core.Models.Settings;

public class ServerSettings : IServerSettings
{
    public int Port { get; set; }

    public ServerSettings()
    {
    }

    public ServerSettings(int port)
    {
        Port = port;
    }
}
