using Ftp.Core.Interfaces.Settings;

namespace Ftp.Core.Models.Settings;

public class ServerSettings : IServerSettings
{
    public int Port { get; }

    public string PassivePortsRange { get; }

    public ServerSettings()
    {
    }

    public ServerSettings(int port, string passivePortRange)
    {
        Port = port;
        PassivePortsRange = passivePortRange;
    }
}
