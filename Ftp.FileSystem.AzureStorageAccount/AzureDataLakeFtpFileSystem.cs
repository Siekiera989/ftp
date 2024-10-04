using Azure.Storage.Blobs;
using Azure.Storage.Files.DataLake;
using Ftp.Core.FileSystem;
using Ftp.Core.Identity;
using Serilog;

namespace Ftp.FileSystem.AzureStorageAccount;

public class AzureDataLakeFtpFilesystem : IFtpFilesystem
{
    private readonly DataLakeFileSystemClient _fileSystemClient;
    private readonly string _userName;
    private readonly ILogger _logger;

    public AzureDataLakeFtpFilesystem(DataLakeFileSystemClient fileSystemClient, string userName, ILogger logger)
    {
        _fileSystemClient = fileSystemClient;
        _userName = userName;
        _logger = logger;
    }

    /// <summary>
    /// Checks whether a directory exists.
    /// </summary>
    /// <param name="path">The directory to check.</param>
    /// <returns>Whether the directory exists.</returns>
    public bool DirectoryExists(string path)
    {
        var directoryClient = _fileSystemClient.GetDirectoryClient($@"{_userName}/{path}");
        return directoryClient.Exists();
    }

    /// <summary>
    /// Checks whether a file exists.
    /// </summary>
    /// <param name="path">The file to check.</param>
    /// <returns>Whether the file exists.</returns>
    public bool FileExists(string path)
    {
        var fileClient = _fileSystemClient.GetFileClient($@"{_userName}/{path}");
        return fileClient.Exists();
    }

    /// <summary>
    /// Retrieves all the files in the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>A list of files.</returns>
    public string[] GetFiles(string path)
    {
        _logger.Information("[{scope}] Started listing directory: [{directory}]", nameof(AzureDataLakeFtpFilesystem), path);

        var directoryClient = _fileSystemClient.GetDirectoryClient($@"{_userName}/{path}");
        var result =  directoryClient.GetPaths(false)
            .Where(p => (bool)!p.IsDirectory)
            .Select(p => p.Name)
            .ToArray();

        _logger.Information("[{scope}] Successfully finished listing directory: [{directory}]", nameof(AzureDataLakeFtpFilesystem), path);

        return result;
    }

    /// <summary>
    /// Retrieves all the subdirectories in the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>A list of directories.</returns>
    public string[] GetSubdirectories(string path)
    {
        _logger.Information("[{scope}] Started listing directory: [{directory}]", nameof(AzureDataLakeFtpFilesystem), path);
        var directoryClient = _fileSystemClient.GetDirectoryClient($@"{_userName}/{path}");

        var directories = directoryClient.GetPaths(false)
            .Where(p => (bool)p.IsDirectory)
            .Select(p => p.Name)
            .RemoveRootCatalog();

        _logger.Information("[{scope}] Successfully finished listing directory: [{directory}]", nameof(AzureDataLakeFtpFilesystem), path);

        return directories;
    }

    /// <summary>
    /// Retrieves a <see cref="IDirectoryEntry"/> object that contains information about a specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>An object containing information about the directory.</returns>
    public async Task<IDirectoryEntry> GetDirectoryInfo(string path)
    {
        _logger.Information("[{scope}] Started getting directory info: [{directory}]", nameof(AzureDataLakeFtpFilesystem), path);

        var directoryClient = _fileSystemClient.GetDirectoryClient($@"{_userName}/{path}");
        if (!await directoryClient.ExistsAsync()) return null;

        var properties = await directoryClient.GetPropertiesAsync();
        return new AzureDirectoryEntry(path, properties.Value.LastModified, properties.Value.CreatedOn, properties.Value.Permissions);
    }

    /// <summary>
    /// Retrieves a <see cref="IFileEntry"/> object that contains information about a specified file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>An object containing information about the file.</returns>
    public async Task<IFileEntry> GetFileInfo(string path)
    {
        _logger.Information("[{scope}] Started listing directory: [{directory}]", nameof(AzureDataLakeFtpFilesystem), path);

        var fileClient = _fileSystemClient.GetFileClient(path);
        if (!await fileClient.ExistsAsync()) return null;

        var properties = await fileClient.GetPropertiesAsync();
        return new AzureFileEntry(fileClient.Name, properties.Value.ContentLength, properties.Value.LastModified, properties.Value.CreatedOn, properties.Value.Permissions);
    }

    /// <summary>
    /// Retrieves a read-only file stream for the specified file.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <returns>A read-only file stream.</returns>
    public async Task<Stream> GetFileStreamAsync(string path)
    {
        try
        {
            _logger.Information("[{scope}] Started reading file: [{file}]", nameof(AzureDataLakeFtpFilesystem), path);

            var fileClient = _fileSystemClient.GetFileClient($@"{_userName}/{path}");
            if (!await fileClient.ExistsAsync()) return null;

            var response = await fileClient.OpenReadAsync();

            _logger.Information("[{scope}] Successfully finished reading file: [{file}]", nameof(AzureDataLakeFtpFilesystem), path);

            return response;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occurred while executing operation: [{operation}]", nameof(AzureDataLakeFtpFilesystem), nameof(GetFileStreamAsync));
            throw;
        }
    }

    /// <summary>
    /// Deletes a file.
    /// </summary>
    /// <param name="path">The path of the file to delete.</param>
    public async Task DeleteFile(string path)
    {
        try
        {
            _logger.Information("[{scope}] Started deleting file or directory metadata: [{file}]", nameof(AzureDataLakeFtpFilesystem), path);

            var fileClient = _fileSystemClient.GetFileClient($@"{_userName}{path}");
            await fileClient.DeleteIfExistsAsync();

            _logger.Information("[{scope}] Successfully finished deleting file or directory metadata: [{file}]", nameof(AzureDataLakeFtpFilesystem), path);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occurred while executing operation", nameof(AzureDataLakeFtpFilesystem));
            throw;
        }
        
    }

    /// <summary>
    /// Deletes a directory recursively, removing all files and subdirectories.
    /// </summary>
    /// <param name="path">The path of the directory to delete.</param>
    public async Task DeleteDirectory(string path)
    {
        try
        {
            _logger.Information("[{scope}] Started deleting file or directory metadata: [{file}]", nameof(AzureDataLakeFtpFilesystem), path);

            var directoryClient = _fileSystemClient.GetDirectoryClient($@"{_userName}{path}");
            await directoryClient.DeleteIfExistsAsync(true);

            _logger.Information("[{scope}] Successfully finished deleting file or directory metadata: [{file}]", nameof(AzureDataLakeFtpFilesystem), path);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occurred while executing operation", nameof(AzureDataLakeFtpFilesystem));
            throw;
        }
        
    }

    /// <summary>
    /// Creates a new directory.
    /// </summary>
    /// <param name="path">The name of the directory to create.</param>
    public void CreateDirectory(string path)
    {
        try
        {
            _logger.Information("[{scope}] Started creating new directory: [{directory}]", nameof(AzureDataLakeFtpFilesystem), path);

            var directoryClient = _fileSystemClient.GetDirectoryClient($"{_userName}{path}");
            directoryClient.CreateIfNotExists();

            _logger.Information("[{scope}] Successfully creating uploading new directory: [{directory}]", nameof(AzureDataLakeFtpFilesystem), path);

        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occurred while executing operation", nameof(AzureDataLakeFtpFilesystem));
            throw;
        }
        
    }

    /// <summary>
    /// Creates a new file.
    /// </summary>
    /// <param name="path">The path of the file to create.</param>
    /// <returns>A file stream for writing to the new file.</returns>
    public async Task CreateFile(string path, Stream fileToUpload)
    {
        try
        {
            _logger.Information("[{scope}] Started uploading new file: {path}", nameof(AzureDataLakeFtpFilesystem), path);

            path = NormalizePath($"{_userName}{path}");
            var fileClient = _fileSystemClient.GetFileClient(path);
            await fileClient.CreateAsync();

            var t = await fileClient.UploadAsync(fileToUpload, true);

            _logger.Information("[{scope}] Successfully finished uploading new file: {path}", nameof(AzureDataLakeFtpFilesystem), path);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occurred while executing operation", nameof(AzureDataLakeFtpFilesystem));
            throw;
        }
        
    }

    /// <summary>
    /// Moves a directory, changing its path.
    /// </summary>
    /// <param name="oldPath">The current path of the directory.</param>
    /// <param name="newPath">The new path of the directory.</param>
    public async Task MoveDirectory(string oldPath, string newPath)
    {
        try
        {
            _logger.Information("[{scope}] Started moving entry from [{sourceDirectory}] to [{targetDirectory}]", nameof(AzureDataLakeFtpFilesystem), oldPath, newPath);

            var oldDirectoryClient = _fileSystemClient.GetDirectoryClient($@"{_userName}{oldPath}");
            var newDirectoryClient = _fileSystemClient.GetDirectoryClient($@"{_userName}{newPath}");
            await oldDirectoryClient.RenameAsync($@"{_userName}{newPath}");

            _logger.Information("[{scope}] Successfully finished moving entry from [{sourceDirectory}] to [{targetDirectory}]", nameof(AzureDataLakeFtpFilesystem), oldPath, newPath);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occurred while executing operation: {operation}", nameof(AzureDataLakeFtpFilesystem), nameof(MoveFile));
            throw;
        }
        
    }

    /// <summary>
    /// Moves a file, changing its path.
    /// </summary>
    /// <param name="oldPath">The current path of the file.</param>
    /// <param name="newPath">The new path of the file.</param>
    public async Task MoveFile(string oldPath, string newPath)
    {
        try
        {
            _logger.Information("[{scope}] Started moving entry from [{sourceDirectory}] to [{targetDirectory}]", nameof(AzureDataLakeFtpFilesystem), oldPath, newPath);

            var fileClient = _fileSystemClient.GetFileClient($@"{_userName}/{oldPath}");
            await fileClient.RenameAsync($@"{_userName}/{newPath}");

            _logger.Information("[{scope}] Successfully finished moving entry from [{sourceDirectory}] to [{targetDirectory}]", nameof(AzureDataLakeFtpFilesystem), oldPath, newPath);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occurred while executing operation: {operation}", nameof(AzureDataLakeFtpFilesystem), nameof(MoveFile));
            throw;
        }
        
    }

    private static string NormalizePath(string path) => path.Replace(@"\", "/");
}

public class AzureDirectoryEntry : IDirectoryEntry
{
    public string Name { get; }
    public string Permissions { get; }
    public DateTimeOffset LastWriteTime { get; }
    public DateTimeOffset CreatedTime { get; }
    public bool IsRoot { get; }
    public bool IsDeletable { get; }
    public long Length => 0;

    public AzureDirectoryEntry(string name, DateTimeOffset lastWriteTime, DateTimeOffset createdTime, string permissions)
    {
        Name = name;
        LastWriteTime = lastWriteTime;
        CreatedTime = createdTime;
        IsRoot = name == "/";
        IsDeletable = !IsRoot;
        Permissions = "drwxrwxrwx";
    }
}

public class AzureFileEntry : IFileEntry
{
    public string Name { get; }
    public string Permissions { get; }
    public DateTimeOffset LastWriteTime { get; }
    public DateTimeOffset CreatedTime { get; }
    public long Length { get; set; }

    public AzureFileEntry(string name, long length, DateTimeOffset lastWriteTime, DateTimeOffset createdTime, string permissions)
    {
        Name = name;
        Length = length;
        LastWriteTime = lastWriteTime;
        CreatedTime = createdTime;
        Permissions = "-rwxrwxrwx";
    }
}

public static class IEnumerableExtensions
{
    public static string[] RemoveRootCatalog(this IEnumerable<string> input)
    {
        return input
            .Select(str =>
            {
                var slashIndex = str.IndexOf('/');
                return slashIndex >= 0 ? str[(slashIndex + 1)..] : str;
            })
            .ToArray();
    }
}