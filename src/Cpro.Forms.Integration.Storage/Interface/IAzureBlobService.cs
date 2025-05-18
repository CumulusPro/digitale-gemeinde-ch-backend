using Azure.Storage.Blobs;

namespace Cpro.Forms.Integration.Storage.Services;

public interface IAzureBlobService
{
    public BlobClient GetBlobClient(string fileName);
    Task<string> GetFile(string fileName);
    string GetSignedUrl(string fileName);
    Task DeleteFile(string fileName);
    Task DeleteFolder(string folderPrefix);
    public Task UploadFile(string uploadFileUrl, byte[] data);
    public Task UploadFile(string fileName, MemoryStream memoryStream);
}