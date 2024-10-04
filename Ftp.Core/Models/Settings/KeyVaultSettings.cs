using Ftp.Core.Interfaces.Settings;

namespace Ftp.Core.Models.Settings;

public class KeyVaultSettings : IKeyVaultSettings
{
    public string KeyVaultUri { get; set; }

    public KeyVaultSettings()
    {        
    }

    public KeyVaultSettings(string keyVaultUri)
    {
        KeyVaultUri = keyVaultUri;
    }
}
