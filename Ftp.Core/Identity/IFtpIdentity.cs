using Ftp.Core.FileSystem;

namespace Ftp.Core.Identity;
public interface IFtpIdentity
{
    /// <summary>
    /// The filesystem that users with this identity should be allowed to interact with.
    /// </summary>
    public IFtpFilesystem Filesystem { get; }
}
