using Ftp.Core.Identity;

namespace Ftp.Core.Interfaces.Settings;

public interface IAuthenticationSettings
{
    MembershipProviderType MembershipProvider { get; set; }
}