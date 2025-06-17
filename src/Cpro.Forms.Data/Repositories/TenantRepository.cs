using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models.Tenant;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;

/// <summary>
/// Repository for managing tenant operations including creation, updates, deletion, and retrieval.
/// </summary>
public class TenantRepository : RepositoryBase<Tenant, SqlContext>, ITenantRepository
{
    public TenantRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    /// <summary>
    /// Creates a new tenant in the database.
    /// </summary>
    /// <param name="tenant">The tenant to create</param>
    /// <returns>The created tenant</returns>
    public async Task<Tenant> CreateTenantAsync(Tenant tenant)
    {
        return await Insert(tenant);
    }

    /// <summary>
    /// Deletes a tenant from the database.
    /// </summary>
    /// <param name="tenant">The tenant to delete</param>
    public async Task DeleteTenantAsync(Tenant tenant)
    {
        await Delete(tenant);
    }

    /// <summary>
    /// Retrieves a tenant by their unique identifier.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant</param>
    /// <returns>The tenant if found; otherwise null</returns>
    public async Task<Tenant?> GetTenantByIdAsync(int tenantId)
    {
        return await Get(x => x.TenantId == tenantId);
    }

    /// <summary>
    /// Searches for tenants based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria including tenant name</param>
    /// <returns>A paged response containing matching tenants</returns>
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

    /// <summary>
    /// Updates an existing tenant in the database.
    /// </summary>
    /// <param name="tenant">The updated tenant information</param>
    /// <returns>The updated tenant</returns>
    public async Task<Tenant> UpdateTenantAsync(Tenant tenant)
    {
        return await Update(tenant);
    }
}
