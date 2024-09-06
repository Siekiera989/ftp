namespace Ftp.Core.FileSystem;

public interface IDirectoryEntry : IFileEntry
{ 
    /// <summary>
    /// Gets a value indicating whether this is the root directory.
    /// </summary>
    bool IsRoot { get; }

    //
    // Summary:
    //     Gets a value indicating whether this directory can be deleted.
    bool IsDeletable { get; }
}
