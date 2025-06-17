using Azure.Storage.Blobs;
using Cpro.Forms.Integration.Storage.Configuration;

namespace Cpro.Forms.Integration.Storage.Services;

/// <summary>
/// Service for managing on-premises file storage operations as an alternative to Azure Blob Storage.
/// </summary>
public class OnPremStorageService : IAzureBlobService
{
    private readonly string _basePath;

    public OnPremStorageService(IStorageConfiguration configuration)
    {
        _basePath = Path.GetFullPath(configuration.OnPremStoragePath);
        Directory.CreateDirectory(_basePath);
    }

    /// <summary>
    /// Deletes a file from the on-premises storage if it exists.
    /// </summary>
    /// <param name="fileName">The name of the file to delete</param>
    /// <returns>A completed task</returns>
    public Task DeleteFile(string fileName)
    {
        var fullPath = GetFullPath(fileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a folder and all its contents from the on-premises storage if it exists.
    /// </summary>
    /// <param name="folderPrefix">The folder prefix to delete</param>
    /// <returns>A completed task</returns>
    public Task DeleteFolder(string folderPrefix)
    {
        var fullPath = GetFullPath(folderPrefix);
        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, recursive: true);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets a blob client. Not supported for on-premises storage.
    /// </summary>
    /// <param name="fileName">The name of the file</param>
    /// <returns>Throws NotSupportedException</returns>
    /// <exception cref="NotSupportedException">Always thrown as this operation is not supported for on-premises storage</exception>
    public BlobClient GetBlobClient(string fileName)
    {
        throw new NotSupportedException("GetBlobClient is not supported for OnPrem storage.");
    }

    /// <summary>
    /// Retrieves the content of a file from on-premises storage.
    /// </summary>
    /// <param name="fileName">The name of the file to retrieve</param>
    /// <returns>The file content as a string, or null if the file doesn't exist</returns>
    public Task<string> GetFile(string fileName)
    {
        var fullPath = GetFullPath(fileName);
        if (!File.Exists(fullPath))
            return Task.FromResult<string>(null);

        var content = File.ReadAllText(fullPath);
        return Task.FromResult(content);
    }

    /// <summary>
    /// Generates a file URL for accessing a file in on-premises storage.
    /// </summary>
    /// <param name="fileName">The name of the file to generate a URL for</param>
    /// <returns>A file URL if the file exists, or empty string if it doesn't</returns>
    public string GetSignedUrl(string fileName)
    {
        var fullPath = GetFullPath(fileName);

        return File.Exists(fullPath)
            ? new Uri(fullPath).AbsoluteUri
            : string.Empty;
    }

    /// <summary>
    /// Uploads byte array data to a specified URL path in on-premises storage.
    /// </summary>
    /// <param name="uploadFileUrl">The URL path to upload the file to</param>
    /// <param name="data">The byte array data to upload</param>
    /// <returns>A completed task</returns>
    public Task UploadFile(string uploadFileUrl, byte[] data)
    {
        var path = new Uri(uploadFileUrl).LocalPath;
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        return File.WriteAllBytesAsync(path, data);
    }

    /// <summary>
    /// Uploads a memory stream to on-premises storage with the specified file name.
    /// </summary>
    /// <param name="fileName">The name of the file in on-premises storage</param>
    /// <param name="memoryStream">The memory stream containing the file data</param>
    /// <returns>A completed task</returns>
    public Task UploadFile(string fileName, MemoryStream memoryStream)
    {
        var fullPath = GetFullPath(fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

        using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        memoryStream.Position = 0;
        memoryStream.CopyTo(fileStream);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Converts a relative path to a full path within the on-premises storage base directory.
    /// </summary>
    /// <param name="relativePath">The relative path to convert</param>
    /// <returns>The full path within the on-premises storage directory</returns>
    private string GetFullPath(string relativePath)
    {
        return Path.Combine(_basePath, relativePath.Replace('/', Path.DirectorySeparatorChar));
    }
}
