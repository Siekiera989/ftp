using Azure.Storage.Blobs;
using Azure.Storage.Files.DataLake;
using Ftp.Core.FileSystem;
using Ftp.Core.Identity;
using Ftp.FileSystem.AzureStorageAccount;
using Serilog;

namespace Ftp.Identity.Default;

public class DefaultIdentity(string username, ILogger logger, BlobContainerClient blobContainerClient, DataLakeFileSystemClient dataLakeFileSystem) : IFtpIdentity
{
    public IFtpFilesystem Filesystem { get; set; } = new StorageAccountFileSystem(blobContainerClient, dataLakeFileSystem, username, logger);
}
