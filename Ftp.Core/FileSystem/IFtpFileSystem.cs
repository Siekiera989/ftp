using Ftp.Core.Identity;

namespace Ftp.Core.FileSystem;

/// <summary>
/// Represents an abstract filesystem that FTP users can interact with.
/// </summary>
public interface IFtpFilesystem
{
    /// <summary>
    /// Checks whether a directory exists.
    /// </summary>
    /// <param name="path">The directory to check.</param>
    /// <returns>Whether the directory exists.</returns>
    bool DirectoryExists(string path);

    /// <summary>
    /// Checks whether a file exists.
    /// </summary>
    /// <param name="path">The file to check.</param>
    /// <returns>Whether the file exists.</returns>
    bool FileExists(string path);

    /// <summary>
    /// Retrieves all the files in the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>A list of files.</returns>
    string[] GetFiles(string path);

    /// <summary>
    /// Retrieves all the subdirectories in the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>A list of directories.</returns>
    string[] GetSubdirectories(string path);

    /// <summary>
    /// Retrieves a <see cref="DirectoryInfo"/> object that contains information about a specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>An object containing information about the directory.</returns>
    Task<IDirectoryEntry> GetDirectoryInfo(string path);

    /// <summary>
    /// Retrieves a <see cref="FileInfo"/> object that contains information about a specified file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>An object containing information about the file.</returns>
    Task<IFileEntry> GetFileInfo(string path);

    /// <summary>
    /// Retrieves a read-only file stream for the specified file.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <returns>A read-only file stream.</returns>
    Task<Stream> GetFileStreamAsync(string path);

    /// <summary>
    /// Deletes a file.
    /// </summary>
    /// <param name="path">The path of the file to delete.</param>
    Task DeleteFile(string path);

    /// <summary>
    /// Deletes a directory recursively, removing all files and subdirectories.
    /// </summary>
    /// <param name="path">The path of the directory to delete.</param>
    Task DeleteDirectory(string path);

    /// <summary>
    /// Creates a new directory.
    /// </summary>
    /// <param name="path">The name of the directory to create.</param>
    void CreateDirectory(string path);

    /// <summary>
    /// Creates a new file.
    /// </summary>
    /// <param name="path">The path of the file to create.</param>
    /// <returns>A file stream for writing to the new file.</returns>
    Task CreateFile(string path, Stream fileToUpload);

    /// <summary>
    /// Moves a directory, changing its path.
    /// </summary>
    /// <param name="oldPath">The current path of the directory.</param>
    /// <param name="newPath">The new path of the directory.</param>
    Task MoveDirectory(string oldPath, string newPath);

    /// <summary>
    /// Moves a file, changing its path.
    /// </summary>
    /// <param name="oldPath">The current path of the file.</param>
    /// <param name="newPath">The new path of the file.</param>
    Task MoveFile(string oldPath, string newPath);
}