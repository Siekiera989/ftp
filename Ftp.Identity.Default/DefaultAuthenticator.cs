using Azure.Storage.Blobs;
using Azure.Storage.Files.DataLake;
using Ftp.Core.Identity;
using Serilog;

namespace Ftp.Identity.Default;

public class DefaultAuthenticator(ILogger logger, BlobContainerClient blobContainerClient, DataLakeFileSystemClient dataLakeFileSystem) : IFtpAuthenticator
{
    private readonly ILogger _logger = logger;
    private readonly BlobContainerClient _blobContainerClient = blobContainerClient;
    private readonly DataLakeFileSystemClient _dataLakeFileSystem = dataLakeFileSystem;

    public IFtpIdentity AuthenticateUser(string username, string password) 
    {
        
        return new DefaultIdentity(username, _logger, _blobContainerClient, _dataLakeFileSystem);
    }
}
