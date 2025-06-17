using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Service.Models;
using Newtonsoft.Json;
using System.Text;

namespace Cpro.Forms.Service.Services;

/// <summary>
/// Service for managing tenant-specific design configurations including header, footer, and design settings.
/// </summary>
public class TenantDesignService : ITenantDesignService
{
    private readonly IAzureBlobService _azureBlobService;

    public TenantDesignService(IAzureBlobService azureBlobService)
    {
        _azureBlobService = azureBlobService;
    }

    /// <summary>
    /// Creates or updates a tenant design configuration and stores it in blob storage.
    /// </summary>
    /// <param name="design">The tenant design configuration to save</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The saved tenant design configuration</returns>
    public async Task<TenantDesign> CreateUpdateTenantDesign(TenantDesign design, int tenantId)
    {
        var jsonObjectString = JsonConvert.SerializeObject(design);
        var jsonObjectBytes = Encoding.UTF8.GetBytes(jsonObjectString);
        using (var memoryStream = new MemoryStream(jsonObjectBytes))
        {
            memoryStream.Position = 0;

            await _azureBlobService.UploadFile($"{tenantId}.json", memoryStream);
        }
        return design;
    }

    /// <summary>
    /// Retrieves a tenant design configuration by tenant ID.
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The tenant design configuration if found; otherwise a new empty configuration</returns>
    public async Task<TenantDesign> GetTenantDesign(int tenantId)
    {
        var blobClient = _azureBlobService.GetBlobClient($"{tenantId}.json");

        if (await blobClient.ExistsAsync())
        {
            var response = await blobClient.DownloadContentAsync();

            var tenantDesign = response.Value.Content.ToObjectFromJson<TenantDesign>();

            if (tenantDesign != null)
            {
                return tenantDesign;
            }
        }
        return new TenantDesign();
    }
}
