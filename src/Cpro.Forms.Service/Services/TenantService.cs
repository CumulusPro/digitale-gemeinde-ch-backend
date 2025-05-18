using AutoMapper;
using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Service.Interface;
using Cpro.Forms.Service.Models.Tenant;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Services;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;

    public TenantService(ITenantRepository tenantRepository, IMapper mapper)
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
    }

    public async Task<TenantResponse> CreateTenantAsync(TenantRequest request)
    {
        var dbTenant = _mapper.Map<Data.Models.Tenant.Tenant>(request);
        var saved = await _tenantRepository.CreateTenantAsync(dbTenant);
        return _mapper.Map<TenantResponse>(saved);
    }

    public async Task DeleteTenantAsync(int id)
    {
        var tenant = await _tenantRepository.GetTenantByIdAsync(id)
            ?? throw new KeyNotFoundException($"Tenant not found with id: {id}");

        await _tenantRepository.DeleteTenantAsync(tenant);
    }

    public async Task<TenantResponse?> GetTenantByIdAsync(int id)
    {
        var tenant = await _tenantRepository.GetTenantByIdAsync(id);
        return _mapper.Map<TenantResponse?>(tenant);
    }

    public async Task<PagingResponse<TenantResponse>> SearchTenants(TenantSearchRequest searchRequest)
    {
        var tenants = await _tenantRepository.SearchTenants(_mapper.Map<Data.Models.Tenant.TenantSearchRequest>(searchRequest));
        return _mapper.Map<PagingResponse<TenantResponse>>(tenants);
    }

    public async Task<TenantResponse> UpdateTenantAsync(int id, TenantRequest request)
    {
        var tenant = await _tenantRepository.GetTenantByIdAsync(id)
            ?? throw new KeyNotFoundException($"Tenant not found with id: {id}");

        tenant.TenantName = request.TenantName;
        var updated = await _tenantRepository.UpdateTenantAsync(tenant);
        return _mapper.Map<TenantResponse>(updated);
    }
}
