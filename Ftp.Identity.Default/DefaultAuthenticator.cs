using Azure.Storage.Files.DataLake;
using Ftp.Core.Identity;
using Serilog;

namespace Ftp.Identity.Default;

public class DefaultAuthenticator(ILogger logger, DataLakeFileSystemClient dataLakeFileSystem) : FtpAuthenticationBase(logger)
{
    private readonly ILogger _logger = logger;
    private readonly DataLakeFileSystemClient _dataLakeFileSystem = dataLakeFileSystem;

    public override IFtpIdentity AuthenticateUser(string username, string password) 
    {
        return new DefaultIdentity(username, _logger, _dataLakeFileSystem);
    }
}
