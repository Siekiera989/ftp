using Serilog;

namespace Ftp.Core.Identity;
public abstract class FtpAuthenticationBase(ILogger logger) : IFtpAuthenticator
{
    public ILogger Logger { get; } = logger;

    public abstract IFtpIdentity AuthenticateUser(string username, string password);
}
