using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Service.Models;
using Newtonsoft.Json;
using System.Text;

namespace Cpro.Forms.Service.Services;

public class TenantDesignService : ITenantDesignService
{
    private readonly IAzureBlobService _azureBlobService;
    private const string GroupName = "Formulare";

    public TenantDesignService(IAzureBlobService azureBlobService)
    {
        _azureBlobService = azureBlobService;
    }


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
