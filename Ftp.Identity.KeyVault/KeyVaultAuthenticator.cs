using System.Security.Claims;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Files.DataLake;
using Ftp.Core.Identity;
using Ftp.Identity.KeyVault.Extensions;
using Ftp.Identity.KeyVault.Factories;
using Serilog;

namespace Ftp.Identity.KeyVault;

public class KeyVaultAuthenticator(ILogger logger, DataLakeFileSystemClient dataLakeFileSystem, SecretClient secretClient) : FtpAuthenticationBase(logger)
{
    private readonly DataLakeFileSystemClient _dataLakeFileSystem = dataLakeFileSystem;
    private readonly SecretClient _secretClient = secretClient;

    public override IFtpIdentity? AuthenticateUser(string username, string password)
    {
        var validationResult = Validate(username, password);
        return validationResult.IsSuccess ? new KeyVaultIdentity(username.ToLowerInvariant(), Logger, _dataLakeFileSystem) : null;
    }

    private MemberValidationResult Validate(string username, string password)
    {
        if (string.IsNullOrEmpty(username))
        {
            return new MemberValidationResult(MemberValidationStatus.InvalidLogin);
        }

        var passwordIsValid = ValidatePassword(username, password);

        if (!passwordIsValid)
        {
            return new MemberValidationResult(MemberValidationStatus.InvalidLogin);
        }

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
                },
                "keyVaultAuth"));

        return new MemberValidationResult(MemberValidationStatus.AuthenticatedUser, user);
    }

    private bool ValidatePassword(string username, string password)
    {
        bool userIsValid = false;

        try
        {
            var secretPassword = _secretClient.GetSecretFromKeyVault($"{KeyVaultConstants.FtpPasswordPrefix}-{username.ToUpperFirstLetter()}");

            userIsValid = password == secretPassword;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "[{scope}] Password validation failed", nameof(KeyVaultAuthenticator));
        }

        return userIsValid;
    }
}
