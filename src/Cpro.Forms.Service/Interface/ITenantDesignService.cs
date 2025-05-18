using Cpro.Forms.Service.Models;

namespace Cpro.Forms.Service.Services;

public interface ITenantDesignService
{
    public Task<TenantDesign> GetTenantDesign(int tenantId);
    public Task<TenantDesign> CreateUpdateTenantDesign(TenantDesign design, int tenantId);
}