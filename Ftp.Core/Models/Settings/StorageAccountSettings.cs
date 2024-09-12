using Ftp.Core.Interfaces.Settings;

namespace Ftp.Core.Models.Settings;

public class StorageAccountSettings : IStorageAccountSettings
{
    public string FirmwareStorageAccountConnectionString { get; set; }

    public StorageAccountSettings()
    {            
    }

    public StorageAccountSettings(string firmwareStorageAccountConnectionString)
    {
        FirmwareStorageAccountConnectionString = firmwareStorageAccountConnectionString;
    }
}
