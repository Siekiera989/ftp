using Ftp.Core.FileSystem;

namespace Ftp.FileSystem.AzureStorageAccount;

public class StorageAccountFileEntry : IFileEntry
{
    public string Name { get; set; }

    public string Permissions { get; set; }

    public DateTimeOffset LastWriteTime { get; set; }

    public DateTimeOffset CreatedTime { get; set; }

    public long Length { get; set; }
}
