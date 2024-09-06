using Ftp.Core.FileSystem;

namespace Ftp.FileSystem.AzureStorageAccount;

public class StorageAccountDirectoryEntry(string path) : IDirectoryEntry
{
    public bool IsRoot { get; set; }

    public bool IsDeletable => !IsRoot;

    public string Name { get; set; } = path;

    public string Permissions { get; set; }

    public DateTimeOffset LastWriteTime { get; set; }

    public DateTimeOffset CreatedTime { get; set; }

    public long Length { get; set; }
}
