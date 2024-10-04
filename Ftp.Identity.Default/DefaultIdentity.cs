using Azure.Storage.Blobs;
using Azure.Storage.Files.DataLake;
using Ftp.Core.FileSystem;
using Ftp.Core.Identity;
using Ftp.FileSystem.AzureStorageAccount;
using Serilog;

namespace Ftp.Identity.Default;

public class DefaultIdentity(string username, ILogger logger, DataLakeFileSystemClient dataLakeFileSystem) : IFtpIdentity
{
    public string Username { get; } = username;
    public IFtpFilesystem Filesystem { get; set; } = new AzureDataLakeFtpFilesystem(dataLakeFileSystem, username, logger);
}
