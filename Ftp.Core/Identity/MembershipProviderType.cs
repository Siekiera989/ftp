namespace Ftp.Core.Identity;

public enum MembershipProviderType
{
    /// <summary>
    /// Use the membership provider for anonymous users.
    /// </summary>
    Anonymous = 0,

    /// <summary>
    /// Use the Azure Key Vault authentication
    /// </summary>
    KeyVault = 1,
}

