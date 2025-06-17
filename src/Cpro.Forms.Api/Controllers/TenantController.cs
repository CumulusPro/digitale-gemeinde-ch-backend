using Cpro.Forms.Service.Interface;
using Cpro.Forms.Service.Models.Tenant;
using Microsoft.AspNetCore.Mvc;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class TenantController : Controller
{
    private readonly ITenantService _tenantService;

    public TenantController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    /// <summary>
    /// Retrieves a tenant by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the tenant</param>
    /// <returns>Tenant information if found; otherwise NotFound</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var user = await _tenantService.GetTenantByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Creates a new tenant in the system.
    /// </summary>
    /// <param name="tenantRequest">The tenant creation request</param>
    /// <returns>The created tenant information</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TenantRequest tenantRequest)
    {
        var result = await _tenantService.CreateTenantAsync(tenantRequest);
        return Ok(result);
    }

    /// <summary>
    /// updates an existing tenant
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tenantRequest"></param>
    /// <returns>The updated tenant information</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TenantRequest tenantRequest)
    {
        var result = await _tenantService.UpdateTenantAsync(id, tenantRequest);
        return Ok(result);
    }

    /// <summary>
    /// Deletes the tenant with the specified identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>NoConetent</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _tenantService.DeleteTenantAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Searches for tenants based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria</param>
    /// <returns>Paged response containing matching tenants</returns>
    [HttpPost("search")]
    public async Task<ActionResult<PagingResponse<TenantResponse>>> SearchTenants([FromBody] TenantSearchRequest searchRequest)
    {
        var users = await _tenantService.SearchTenants(searchRequest);
        return Ok(users);
    }
}
