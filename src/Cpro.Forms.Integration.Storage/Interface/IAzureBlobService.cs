using Azure.Storage.Blobs;

namespace Cpro.Forms.Integration.Storage.Services;

/// <summary>
/// Interface for Azure Blob Storage operations including file upload, download, deletion, and URL generation.
/// </summary>
public interface IAzureBlobService
{
    /// <summary>
    /// Gets a blob client for the specified file name.
    /// </summary>
    /// <param name="fileName">The name of the file in blob storage</param>
    /// <returns>A blob client for the specified file</returns>
    public BlobClient GetBlobClient(string fileName);
    
    /// <summary>
    /// Retrieves the content of a file from Azure Blob Storage.
    /// </summary>
    /// <param name="fileName">The name of the file to retrieve</param>
    /// <returns>The file content as a string, or null if the file doesn't exist</returns>
    Task<string> GetFile(string fileName);
    
    /// <summary>
    /// Generates a signed URL for accessing a blob with read permissions for 24 hours.
    /// </summary>
    /// <param name="fileName">The name of the file to generate a signed URL for</param>
    /// <returns>A signed URL for accessing the blob, or empty string if generation fails</returns>
    string GetSignedUrl(string fileName);
    
    /// <summary>
    /// Deletes a file from Azure Blob Storage if it exists.
    /// </summary>
    /// <param name="fileName">The name of the file to delete</param>
    Task DeleteFile(string fileName);
    
    /// <summary>
    /// Deletes all blobs with the specified folder prefix from Azure Blob Storage.
    /// </summary>
    /// <param name="folderPrefix">The folder prefix to match for deletion</param>
    Task DeleteFolder(string folderPrefix);
    
    /// <summary>
    /// Uploads byte array data to a specified URL (typically a signed URL from another service).
    /// </summary>
    /// <param name="uploadFileUrl">The URL to upload the file to</param>
    /// <param name="data">The byte array data to upload</param>
    public Task UploadFile(string uploadFileUrl, byte[] data);
    
    /// <summary>
    /// Uploads a memory stream to Azure Blob Storage with the specified file name.
    /// </summary>
    /// <param name="fileName">The name of the file in blob storage</param>
    /// <param name="memoryStream">The memory stream containing the file data</param>
    public Task UploadFile(string fileName, MemoryStream memoryStream);
}