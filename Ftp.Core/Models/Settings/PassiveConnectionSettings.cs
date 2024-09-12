using Ftp.Core.Interfaces.Settings;

namespace Ftp.Core.Models.Settings;

public class PassiveConnectionSettings : IPassiveConnectionSettings
{
    public string PassivePortsRange { get; set; }

    public PassiveConnectionSettings()
    {
    }

    public PassiveConnectionSettings(string passivePortsRange)
    {
        PassivePortsRange = passivePortsRange;
    }
}