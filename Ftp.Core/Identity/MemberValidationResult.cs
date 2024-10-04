using System.Security.Claims;

namespace Ftp.Core.Identity;

public class MemberValidationResult
{
    private readonly MemberValidationStatus _status;

    private readonly ClaimsPrincipal _ftpUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberValidationResult"/> class.
    /// </summary>
    /// <param name="status">The error status for the validation.</param>
    public MemberValidationResult(MemberValidationStatus status)
    {
        if (status == MemberValidationStatus.Anonymous || status == MemberValidationStatus.AuthenticatedUser)
        {
            throw new ArgumentOutOfRangeException(nameof(status), "User object must be specified when validation was successful.");
        }

        _status = status;
    }
    

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberValidationResult"/> class.
    /// </summary>
    /// <param name="status">The success status for the validation.</param>
    /// <param name="user">The validated user.</param>
    public MemberValidationResult(MemberValidationStatus status, ClaimsPrincipal user)
    {
        if (status != MemberValidationStatus.Anonymous && status != MemberValidationStatus.AuthenticatedUser)
        {
            throw new ArgumentOutOfRangeException(nameof(status), "User object must only be specified when validation was successful.");
        }

        _status = status;
        _ftpUser = user ?? throw new ArgumentNullException(nameof(user));
    }

    /// <summary>
    /// Gets a value indicating whether the user login succeeded.
    /// </summary>
    public bool IsSuccess => _status == MemberValidationStatus.Anonymous || _status == MemberValidationStatus.AuthenticatedUser;

    /// <summary>
    /// Gets the FTP user.
    /// </summary>
    public ClaimsPrincipal FtpUser
    {
        get
        {
            if (!IsSuccess)
            {
                throw new InvalidOperationException("User is only available when the authentication was successful.");
            }

            return _ftpUser!;
        }
    }
}
