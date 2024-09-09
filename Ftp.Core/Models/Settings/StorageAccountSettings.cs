using Ftp.Core.Interfaces.Settings;

namespace Ftp.Core.Models.Settings;

public class StorageAccountSettings : IStorageAccountSettings
{
    public StorageAccountSettings()
    {            
    }

    public StorageAccountSettings(string firmwareStorageAccountConnectionString)
    {
        FirmwareStorageAccountConnectionString = firmwareStorageAccountConnectionString;
    }

    public string FirmwareStorageAccountConnectionString { get; }
}
