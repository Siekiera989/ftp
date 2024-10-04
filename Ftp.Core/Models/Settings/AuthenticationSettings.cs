using Ftp.Core.Identity;
using Ftp.Core.Interfaces.Settings;

namespace Ftp.Core.Models.Settings;
public class AuthenticationSettings : IAuthenticationSettings
{
    public MembershipProviderType MembershipProvider { get; set; }

    public AuthenticationSettings()
    {        
    }

    public AuthenticationSettings(MembershipProviderType membershipProvider)
    {
        MembershipProvider = membershipProvider;
    }
}
