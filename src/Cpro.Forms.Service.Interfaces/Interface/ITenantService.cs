using Cpro.Forms.Service.Models.Tenant;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Interface;

public interface ITenantService
{
    Task<TenantResponse> CreateTenantAsync(TenantRequest request);
    Task<TenantResponse?> GetTenantByIdAsync(int id);
    Task<PagingResponse<TenantResponse>> SearchTenants(TenantSearchRequest searchRequest);
    Task<TenantResponse> UpdateTenantAsync(int id, TenantRequest request);
    Task DeleteTenantAsync(int id);
}
