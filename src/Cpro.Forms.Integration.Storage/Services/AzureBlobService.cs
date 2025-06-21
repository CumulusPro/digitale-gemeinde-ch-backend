using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Cpro.Forms.Integration.Storage.Configuration;
using System.Text;

namespace Cpro.Forms.Integration.Storage.Services;

/// <summary>
/// Service for managing Azure Blob Storage operations including file upload, download, deletion, and URL generation.
/// </summary>
public class AzureBlobService : IAzureBlobService
{
    private readonly IStorageConfiguration _configuration;

    public AzureBlobService(IStorageConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets a blob client for the specified file name.
    /// </summary>
    /// <param name="fileName">The name of the file in blob storage</param>
    /// <returns>A blob client for the specified file</returns>
    public BlobClient GetBlobClient(string fileName)
    {
        var azureStorageConnectionString = _configuration.AzureStorageConnectionString;
        var containerName = _configuration.ContainerName;

        BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        return blobClient;
    }

    /// <summary>
    /// Retrieves the content of a file from Azure Blob Storage.
    /// </summary>
    /// <param name="fileName">The name of the file to retrieve</param>
    /// <returns>The file content as a string, or null if the file doesn't exist</returns>
    public async Task<string> GetFile(string fileName)
    {
        BlobClient blobClient = GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
            return null;

        var downloadInfo = await blobClient.DownloadAsync();

        using (var reader = new StreamReader(downloadInfo.Value.Content, Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }
    }

    /// <summary>
    /// Generates a signed URL for accessing a blob with read permissions for 24 hours.
    /// </summary>
    /// <param name="fileName">The name of the file to generate a signed URL for</param>
    /// <returns>A signed URL for accessing the blob, or empty string if generation fails</returns>
    public string GetSignedUrl(string fileName)
    {
        try
        {
            BlobClient blobClient = GetBlobClient(fileName);

            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _configuration.ContainerName,
                BlobName = fileName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(24),
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            Uri sasToken = blobClient.GenerateSasUri(sasBuilder);
            return sasToken.ToString();

        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Deletes a file from Azure Blob Storage if it exists.
    /// </summary>
    /// <param name="fileName">The name of the file to delete</param>
    public async Task DeleteFile(string fileName)
    {
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            BlobClient blobClient = GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }
    }

    /// <summary>
    /// Deletes all blobs with the specified folder prefix from Azure Blob Storage.
    /// </summary>
    /// <param name="folderPrefix">The folder prefix to match for deletion</param>
    public async Task DeleteFolder(string folderPrefix)
    {
        var containerClient = GetBlobContainerClient();
        var deleteTasks = new List<Task>();

        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: folderPrefix))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            deleteTasks.Add(blobClient.DeleteIfExistsAsync());
        }

        await Task.WhenAll(deleteTasks);
    }

    /// <summary>
    /// Uploads byte array data to a specified URL (typically a signed URL from another service).
    /// </summary>
    /// <param name="uploadFileUrl">The URL to upload the file to</param>
    /// <param name="data">The byte array data to upload</param>
    public async Task UploadFile(string uploadFileUrl, byte[] data)
    {
        var blobClient = new BlobClient(new Uri(uploadFileUrl));

        using (var memoryStream = new MemoryStream(data))
        {
            memoryStream.Position = 0;
            await blobClient.UploadAsync(memoryStream, overwrite: true);
        }
    }

    /// <summary>
    /// Uploads a memory stream to Azure Blob Storage with the specified file name.
    /// </summary>
    /// <param name="fileName">The name of the file in blob storage</param>
    /// <param name="memoryStream">The memory stream containing the file data</param>
    public async Task UploadFile(string fileName, MemoryStream memoryStream)
    {
        BlobClient blobClient = GetBlobClient(fileName);
        await blobClient.UploadAsync(memoryStream, overwrite: true);
    }

    /// <summary>
    /// Gets a blob container client for the configured storage account and container.
    /// </summary>
    /// <returns>A blob container client</returns>
    private BlobContainerClient GetBlobContainerClient()
    {
        var azureStorageConnectionString = _configuration.AzureStorageConnectionString;
        var containerName = _configuration.ContainerName;

        var blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        return containerClient;
    }
}
