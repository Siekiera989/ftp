using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Ftp.Identity.KeyVault.Factories;

public static class SecretClientFactory
{
    /// <summary>
    /// Retrieves Azure Key Vault Service using your credentials stored on your computer.
    /// </summary>
    /// <param name="url">Address to the Azure Key Vault Service</param>
    /// <returns>Azure Key Vault Service</returns>
    public static SecretClient Create(string url) => BuildSecretClient(url, new DefaultAzureCredential());

    private static SecretClient BuildSecretClient(string url, TokenCredential tokenCredential)
    {
        var uri = new Uri(url);
        var client = new SecretClient(uri, tokenCredential);

        return client;
    }

    public static string GetSecretFromKeyVault(this SecretClient secretClient, string keyVaultSecretName)
        => secretClient.GetSecret(keyVaultSecretName).Value.Value;
}
