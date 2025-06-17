using AutoMapper;
using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Service.Interface;
using Cpro.Forms.Service.Models.Tenant;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Services;

/// <summary>
/// Service for managing tenant operations including creation, updates, deletion, and retrieval.
/// </summary>
public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;

    public TenantService(ITenantRepository tenantRepository, IMapper mapper)
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new tenant in the system.
    /// </summary>
    /// <param name="request">The tenant request containing tenant details</param>
    /// <returns>The created tenant response</returns>
    public async Task<TenantResponse> CreateTenantAsync(TenantRequest request)
    {
        var dbTenant = _mapper.Map<Data.Models.Tenant.Tenant>(request);
        var saved = await _tenantRepository.CreateTenantAsync(dbTenant);
        return _mapper.Map<TenantResponse>(saved);
    }

    /// <summary>
    /// Deletes a tenant from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the tenant to delete</param>
    /// <exception cref="KeyNotFoundException">Thrown when the tenant is not found</exception>
    public async Task DeleteTenantAsync(int id)
    {
        var tenant = await _tenantRepository.GetTenantByIdAsync(id)
            ?? throw new KeyNotFoundException($"Tenant not found with id: {id}");

        await _tenantRepository.DeleteTenantAsync(tenant);
    }

    /// <summary>
    /// Retrieves a tenant by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the tenant</param>
    /// <returns>The tenant response if found; otherwise null</returns>
    public async Task<TenantResponse?> GetTenantByIdAsync(int id)
    {
        var tenant = await _tenantRepository.GetTenantByIdAsync(id);
        return _mapper.Map<TenantResponse?>(tenant);
    }

    /// <summary>
    /// Searches for tenants based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria</param>
    /// <returns>A paged response containing matching tenants</returns>
    public async Task<PagingResponse<TenantResponse>> SearchTenants(TenantSearchRequest searchRequest)
    {
        var tenants = await _tenantRepository.SearchTenants(_mapper.Map<Data.Models.Tenant.TenantSearchRequest>(searchRequest));
        return _mapper.Map<PagingResponse<TenantResponse>>(tenants);
    }

    /// <summary>
    /// Updates an existing tenant's information.
    /// </summary>
    /// <param name="id">The unique identifier of the tenant to update</param>
    /// <param name="request">The updated tenant information</param>
    /// <returns>The updated tenant response</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the tenant is not found</exception>
    public async Task<TenantResponse> UpdateTenantAsync(int id, TenantRequest request)
    {
        var tenant = await _tenantRepository.GetTenantByIdAsync(id)
            ?? throw new KeyNotFoundException($"Tenant not found with id: {id}");

        tenant.TenantName = request.TenantName;
        var updated = await _tenantRepository.UpdateTenantAsync(tenant);
        return _mapper.Map<TenantResponse>(updated);
    }
}
