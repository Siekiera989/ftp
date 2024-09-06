using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.DataLake;
using Ftp.Core.FileSystem;
using Ftp.Core.Identity;
using Serilog;

namespace Ftp.FileSystem.AzureStorageAccount;

/// <summary>
/// Represents a filesystem where users have full control over any files under a certain root directory.
/// </summary>
/// <remarks>
/// Creates a new <see cref="AnonymousFilesystem"/> instance with the specified data.
/// </remarks>
/// <param name="rootDirectory">The root directory in which users should be allowed to edit files.</param>
public class StorageAccountFileSystem(BlobContainerClient blobContainerClient, DataLakeFileSystemClient dataLakeFileSystem, string userName, ILogger logger) : IFtpFilesystem
{
    private readonly BlobContainerClient _blobContainerClient = blobContainerClient;
    private readonly DataLakeFileSystemClient _dataLakeFileSystem = dataLakeFileSystem;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// The root directory in which users should be allowed to edit files.
    /// </summary>
    public string RootDirectory { get; set; } = $"{userName}/";

    /// <summary>
    /// Checks whether a directory exists.
    /// </summary>
    /// <param name="path">The directory to check.</param>
    /// <returns>Whether the directory exists.</returns>
    public virtual bool DirectoryExists(string path)
    {
        path = CombinePaths(RootDirectory, path);

        var directoryClient = _dataLakeFileSystem.GetDirectoryClient(path);
        var directoryProperty = directoryClient.GetProperties();
        return directoryClient.Exists() && directoryProperty.Value.IsDirectory;
    }

    /// <summary>
    /// Checks whether a file exists.
    /// </summary>
    /// 
    /// <param name="path">The file to check.</param>
    /// <returns>Whether the file exists.</returns>
    public virtual bool FileExists(string path)
    {
       path = CombinePaths(RootDirectory, path);

        var fileClient = _dataLakeFileSystem.GetFileClient(path);
        var fileProperty = fileClient.GetProperties();
        return fileClient.Exists() && !fileProperty.Value.IsDirectory;
    }

    /// <summary>
    /// Retrieves all the files in the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>A list of files.</returns>
    public virtual string[] GetFiles(string path)
    {
        path = CombinePaths(RootDirectory, path);

        _logger.Information("[{scope}] Started listing directory: [{directory}]", nameof(StorageAccountFileSystem), path);

        var blobs = _blobContainerClient.GetBlobsByHierarchy(BlobTraits.None, BlobStates.None, delimiter: "/", prefix: path).ToList();

        var result = new List<string>();

        foreach (var blob in blobs)
        {
            if (blob.Blob is not null)
            {
                result.Add(blob.Blob.Name);
            }
        }

        _logger.Information("[{scope}] Successfully finished listing directory: [{directory}]", nameof(StorageAccountFileSystem), path);

        return [.. result];
    }

    /// <summary>
    /// Retrieves all the subdirectories in the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>A list of directories.</returns>
    public virtual string[] GetSubdirectories(string path)
    {
        path = CombinePaths(RootDirectory, path);
        _logger.Information("[{scope}] Started listing directory: [{directory}]", nameof(StorageAccountFileSystem), path);

        var directoryClient = _dataLakeFileSystem.GetDirectoryClient(path);
        var paths = directoryClient.GetPaths().ToList();
        
        var directories = paths.Select(x => x.Name).ToArray();

        //for (int i = 0; i < directories.Length; i++)
        //{
        //    directories[i] = directories[i].Substring(RootDirectory.Length);
        //}

        _logger.Information("[{scope}] Successfully finished listing directory: [{directory}]", nameof(StorageAccountFileSystem), path);

        return directories;
    }

    /// <summary>
    /// Retrieves a <see cref="DirectoryInfo"/> object that contains information about a specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>An object containing information about the directory.</returns>
    public virtual async Task<IDirectoryEntry> GetDirectoryInfo(string path)
    {
        path = CombinePaths(RootDirectory, path);

        _logger.Information("[{scope}] Started getting directory info: [{directory}]", nameof(StorageAccountFileSystem), path);

        var directoryClient = _dataLakeFileSystem.GetDirectoryClient(path);

        var directoryProperty = await directoryClient.GetPropertiesAsync();

        var result = new StorageAccountDirectoryEntry(path)
        {
            IsRoot = false,
            LastWriteTime = directoryProperty.Value.LastModified,
            CreatedTime = directoryProperty.Value.CreatedOn,
            Permissions = directoryProperty.Value.Permissions
        };

        return result;
    }

    /// <summary>
    /// Retrieves a <see cref="FileInfo"/> object that contains information about a specified file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>An object containing information about the file.</returns>
    public virtual async Task<IFileEntry> GetFileInfo(string path)
    {
        path = CombinePaths(RootDirectory, path);
        _logger.Information("[{scope}] Started listing directory: [{directory}]", nameof(StorageAccountFileSystem), path);

        var blobClient = _blobContainerClient.GetBlobClient(path);
        var proprerty = await blobClient.GetPropertiesAsync();

        var result = new StorageAccountFileEntry()
        {
            Name = blobClient.Name,
            LastWriteTime = proprerty.Value.LastModified,
            CreatedTime = proprerty.Value.CreatedOn,
            Length = proprerty.Value.ContentLength
        };

        return result;
    }

    /// <summary>
    /// Retrieves a read-only file stream for the specified file.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <returns>A read-only file stream.</returns>
    public virtual async Task<Stream> GetFileStreamAsync(string path)
    {
       path = CombinePaths(RootDirectory, path);
        try
        {
            _logger.Information("[{scope}] Started reading file: [{file}]", nameof(StorageAccountFileSystem), path);

            var blobClient = _blobContainerClient.GetBlobClient(path);

            var result = await blobClient.OpenReadAsync();

            _logger.Information("[{scope}] Successfully finished reading file: [{file}]", nameof(StorageAccountFileSystem), path);

            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occured while executing operation: [{operation}]", nameof(StorageAccountFileSystem), nameof(GetFileStreamAsync));
            throw;
        }
    }

    /// <summary>
    /// Gets the permission listing for the specified file.
    /// </summary>
    /// <param name="path">The path of the file to get information about.</param>
    /// <param name="identity">The identity of the current user.</param>
    /// <returns>A unix-formatted permission string.</returns>
    public virtual string GetFilePermissions(string path, IFtpIdentity identity)
    {
        return "-rwxrwxrwx";
    }

    /// <summary>
    /// Gets the permission listing for the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory to get information about.</param>
    /// <param name="identity">The identity of the current user.</param>
    /// <returns>A unix-formatted permission string.</returns>
    public virtual string GetDirectoryPermissions(string path, IFtpIdentity identity)
    {
        return "drwxrwxrwx";
    }

    /// <summary>
    /// Deletes a file.
    /// </summary>
    /// <param name="path">The path of the file to delete.</param>
    public virtual async Task DeleteFile(string path)
    {
        path = CombinePaths(RootDirectory, path);
        try
        {
            _logger.Information("[{scope}] Started deleting file or directory metadata: [{file}]", nameof(StorageAccountFileSystem), path);

            var blobClient = _blobContainerClient.GetBlobClient(path);
            await blobClient.DeleteIfExistsAsync();

            _logger.Information("[{scope}] Successfully finished deleting file or directory metadata: [{file}]", nameof(StorageAccountFileSystem), path);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occured while executing operation", nameof(StorageAccountFileSystem));
            throw;
        }
    }

    /// <summary>
    /// Deletes a directory recursively, removing all files and subdirectories.
    /// </summary>
    /// <param name="path">The path of the directory to delete.</param>
    public virtual async Task DeleteDirectory(string path)
    {
        path = CombinePaths(RootDirectory, path);
        try
        {
            _logger.Information("[{scope}] Started deleting file or directory metadata: [{file}]", nameof(StorageAccountFileSystem), path);

            var blobClient = _blobContainerClient.GetBlobClient(path);
            await blobClient.DeleteIfExistsAsync();

            _logger.Information("[{scope}] Successfully finished deleting file or directory metadata: [{file}]", nameof(StorageAccountFileSystem), path);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occured while executing operation", nameof(StorageAccountFileSystem));
            throw;
        }
    }

    /// <summary>
    /// Creates a new directory.
    /// </summary>
    /// <param name="path">The name of the directory to create.</param>
    public virtual void CreateDirectory(string path)
    {
        path = CombinePaths(RootDirectory, path);
        try
        {
            _logger.Information("[{scope}] Started creating new directory: [{directory}]", nameof(StorageAccountFileSystem), path);

            _dataLakeFileSystem.CreateDirectory(path);

            _logger.Information("[{scope}] Successfully creating uploading new directory: [{directory}]", nameof(StorageAccountFileSystem), path);

        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occured while executing operation", nameof(StorageAccountFileSystem));
            throw;
        }
    }

    /// <summary>
    /// Creates a new file.
    /// </summary>
    /// <param name="path">The path of the file to create.</param>
    /// <returns>A file stream for writing to the new file.</returns>
    public virtual async Task CreateFile(string path, Stream fileData)
    {
        path = CombinePaths(RootDirectory, path);

        try
        {
            _logger.Information("[{scope}] Started uploading new file: [{file}]", nameof(StorageAccountFileSystem), path);

            var blobClient = _blobContainerClient.GetBlobClient(path);

            await blobClient.UploadAsync(fileData);

            _logger.Information("[{scope}] Successfully finished uploading new file: [{file}]", nameof(StorageAccountFileSystem), path);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occured while executing operation", nameof(StorageAccountFileSystem));
            throw;
        }
    }

    /// <summary>
    /// Moves a directory, changing its path.
    /// </summary>
    /// <param name="oldPath">The current path of the directory.</param>
    /// <param name="newPath">The new path of the directory.</param>
    public virtual async Task MoveDirectory(string oldPath, string newPath)
    {
        try
        {
            _logger.Information("[{scope}] Started moveing entry from [{sourceDirectory}] to [{targetDirectory}]", nameof(StorageAccountFileSystem), oldPath, newPath);

            var sourceBlobClient = _blobContainerClient.GetBlobClient(oldPath);
            var targetBlobClient = _blobContainerClient.GetBlobClient(newPath);

            var result = await targetBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);

            result.WaitForCompletion();

            sourceBlobClient.DeleteIfExists();

            _logger.Information("[{scope}] Successfully finished moveing entry from [{sourceDirectory}] to [{targetDirectory}]", nameof(StorageAccountFileSystem), oldPath, newPath);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occured while executing operation: {operation}", nameof(StorageAccountFileSystem), nameof(MoveFile));
            throw;
        }
    }

    /// <summary>
    /// Moves a file, changing its path.
    /// </summary>
    /// <param name="oldPath">The current path of the file.</param>
    /// <param name="newPath">The new path of the file.</param>
    public virtual async Task MoveFile(string oldPath, string newPath)
    {
        try
        {
            _logger.Information("[{scope}] Started moveing entry from [{sourceDirectory}] to [{targetDirectory}]", nameof(StorageAccountFileSystem), oldPath, newPath);

            var sourceBlobClient = _blobContainerClient.GetBlobClient(oldPath);
            var targetBlobClient = _blobContainerClient.GetBlobClient(newPath);

            var result = await targetBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);

            result.WaitForCompletion();

            sourceBlobClient.DeleteIfExists();

            _logger.Information("[{scope}] Successfully finished moveing entry from [{sourceDirectory}] to [{targetDirectory}]", nameof(StorageAccountFileSystem), oldPath, newPath);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[{scope}] An exception occured while executing operation: {operation}", nameof(StorageAccountFileSystem), nameof(MoveFile));
            throw;
        }
    }

    private string CombinePaths(params string[] args)
    {
        string output = "";
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].EndsWith("/"))
            {
                output += args[i];
            }
            else
            {
                if (i == args.Length - 1)
                {
                    output += args[i];
                }
                else
                {
                    output += args[i] + "/";
                }
            }
        }
        return output;
    }
}