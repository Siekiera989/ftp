namespace Ftp.Core.FileSystem;

public interface IFileEntry
{
    //
    // Summary:
    //     Gets the name of the file system entry.
    string Name { get; }

    //
    // Summary:
    //     Gets the file entry permissions.
    string Permissions { get; }

    //
    // Summary:
    //     Gets the last write time.
    DateTimeOffset LastWriteTime { get; }

    //
    // Summary:
    //     Gets the time of creation.
    DateTimeOffset CreatedTime { get; }

    long Length { get; }
}
