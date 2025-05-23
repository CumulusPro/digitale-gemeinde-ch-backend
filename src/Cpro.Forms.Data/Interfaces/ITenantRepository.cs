using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models.Tenant;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant> CreateTenantAsync(Tenant tenant);
    Task<Tenant?> GetTenantByIdAsync(int tenantId);
    Task<PagingResponse<Tenant>> SearchTenants(TenantSearchRequest searchRequest);
    Task<Tenant> UpdateTenantAsync(Tenant tenant);
    Task DeleteTenantAsync(Tenant tenant);
}
