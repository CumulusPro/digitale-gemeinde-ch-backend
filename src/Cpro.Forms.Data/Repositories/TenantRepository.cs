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

public class TenantRepository : RepositoryBase<Tenant, SqlContext>, ITenantRepository
{
    public TenantRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    public async Task<Tenant> CreateTenantAsync(Tenant tenant)
    {
        return await Insert(tenant);
    }

    public async Task DeleteTenantAsync(Tenant tenant)
    {
        await Delete(tenant);
    }

    public async Task<Tenant?> GetTenantByIdAsync(int tenantId)
    {
        return await Get(x => x.TenantId == tenantId);
    }

    public async Task<PagingResponse<Tenant>> SearchTenants(TenantSearchRequest searchRequest)
    {
        var query = _context.Tenants.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchRequest.TenantName))
        {
            query = query.Where(t => t.TenantName.Contains(searchRequest.TenantName));
        }

        var totalItemCount = await query.CountAsync();
        var searchResults = await query.ToPagedList(searchRequest);

        return new PagingResponse<Tenant>
        {
            Data = searchResults,
            TotalCount = totalItemCount,
            PageNumber = searchRequest.Page,
            PageSize = searchRequest.PageSize,
        };
    }

    public async Task<Tenant> UpdateTenantAsync(Tenant tenant)
    {
        return await Update(tenant);
    }
}
