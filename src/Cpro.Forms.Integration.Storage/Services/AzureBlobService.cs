using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Cpro.Forms.Integration.Storage.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text;

namespace Cpro.Forms.Integration.Storage.Services;

public class AzureBlobService : IAzureBlobService
{
    private readonly IStorageConfiguration _configuration;

    public AzureBlobService(IStorageConfiguration configuration)
    {
        _configuration = configuration;
    }

    public BlobClient GetBlobClient(string fileName)
    {
        var azureStorageConnectionString = _configuration.AzureStorageConnectionString;
        var containerName = _configuration.ContainerName;

        BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        return blobClient;
    }

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

    public async Task DeleteFile(string fileName)
    {
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            BlobClient blobClient = GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }
    }

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

    public async Task UploadFile(string uploadFileUrl, byte[] data)
    {
        using (var memoryStream = new MemoryStream(data))
        {
            memoryStream.Position = 0;

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(uploadFileUrl));
            await blob.UploadFromStreamAsync(memoryStream);
        }
    }

    public async Task UploadFile(string fileName, MemoryStream memoryStream)
    {
        BlobClient blobClient = GetBlobClient(fileName);
        await blobClient.UploadAsync(memoryStream, overwrite: true);
    }

    private BlobContainerClient GetBlobContainerClient()
    {
        var azureStorageConnectionString = _configuration.AzureStorageConnectionString;
        var containerName = _configuration.ContainerName;

        var blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        return containerClient;
    }
}
