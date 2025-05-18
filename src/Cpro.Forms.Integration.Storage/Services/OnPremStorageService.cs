using Azure.Storage.Blobs;
using Cpro.Forms.Integration.Storage.Configuration;

namespace Cpro.Forms.Integration.Storage.Services;

public class OnPremStorageService : IAzureBlobService
{
    private readonly string _basePath;

    public OnPremStorageService(IStorageConfiguration configuration)
    {
        _basePath = Path.GetFullPath(configuration.OnPremStoragePath);
        Directory.CreateDirectory(_basePath);
    }

    public Task DeleteFile(string fileName)
    {
        var fullPath = GetFullPath(fileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task DeleteFolder(string folderPrefix)
    {
        var fullPath = GetFullPath(folderPrefix);
        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, recursive: true);
        }

        return Task.CompletedTask;
    }

    public BlobClient GetBlobClient(string fileName)
    {
        throw new NotSupportedException("GetBlobClient is not supported for OnPrem storage.");
    }

    public Task<string> GetFile(string fileName)
    {
        var fullPath = GetFullPath(fileName);
        if (!File.Exists(fullPath))
            return Task.FromResult<string>(null);

        var content = File.ReadAllText(fullPath);
        return Task.FromResult(content);
    }

    public string GetSignedUrl(string fileName)
    {
        var fullPath = GetFullPath(fileName);

        return File.Exists(fullPath)
            ? new Uri(fullPath).AbsoluteUri
            : string.Empty;
    }

    public Task UploadFile(string uploadFileUrl, byte[] data)
    {
        var path = new Uri(uploadFileUrl).LocalPath;
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        return File.WriteAllBytesAsync(path, data);
    }

    public Task UploadFile(string fileName, MemoryStream memoryStream)
    {
        var fullPath = GetFullPath(fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

        using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        memoryStream.Position = 0;
        memoryStream.CopyTo(fileStream);
        return Task.CompletedTask;
    }

    private string GetFullPath(string relativePath)
    {
        return Path.Combine(_basePath, relativePath.Replace('/', Path.DirectorySeparatorChar));
    }
}
