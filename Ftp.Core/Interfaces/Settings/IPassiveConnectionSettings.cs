namespace Ftp.Core.Interfaces.Settings;

public interface IPassiveConnectionSettings
{

    /// <summary>
    /// Range format {minPort:maxPort}
    /// </summary>
    string PassivePortsRange { get; }
}
