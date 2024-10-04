using Azure.Storage.Files.DataLake;
using Ftp.Core.FileSystem;
using Ftp.Core.Identity;
using Ftp.FileSystem.AzureStorageAccount;
using Serilog;

namespace Ftp.Identity.KeyVault;
public class KeyVaultIdentity(string username, ILogger logger, DataLakeFileSystemClient dataLakeFileSystem) : IFtpIdentity
{
    public IFtpFilesystem Filesystem { get; } = new AzureDataLakeFtpFilesystem(dataLakeFileSystem, username, logger);

    public string Username => username;
}
