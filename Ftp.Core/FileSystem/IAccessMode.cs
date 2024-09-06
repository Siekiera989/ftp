namespace Ftp.Core.FileSystem;

public interface IAccessMode
{
    //
    // Summary:
    //     Gets a value indicating whether a read is allowed.
    bool Read { get; }

    //
    // Summary:
    //     Gets a value indicating whether a write is allowed.
    bool Write { get; }

    //
    // Summary:
    //     Gets a value indicating whether an execute is allowed.
    bool Execute { get; }
}