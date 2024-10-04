namespace Ftp.Core.Identity;

public enum MemberValidationStatus
{
    /// <summary>
    /// User name or password invalid.
    /// </summary>
    InvalidLogin,

    /// <summary>
    /// Email address validation for anonymous login failed.
    /// </summary>
    InvalidAnonymousEmail,

    /// <summary>
    /// Anonymous user.
    /// </summary>
    Anonymous,

    /// <summary>
    /// Authenticated user.
    /// </summary>
    AuthenticatedUser
}