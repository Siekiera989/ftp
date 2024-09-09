using Ftp.Core.Interfaces.Settings;

namespace Ftp.Tools.App.DependencyInjection;

public class PassiveConnectionSettings : IPassiveConnectionSettings
{
    public string PassivePortsRange { get; }

    public PassiveConnectionSettings()
    {        
    }

    public PassiveConnectionSettings(string passivePortsRange)
    {
        PassivePortsRange = passivePortsRange;
    }
}