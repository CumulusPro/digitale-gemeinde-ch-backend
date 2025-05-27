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

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var user = await _tenantService.GetTenantByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TenantRequest tenantRequest)
    {
        var result = await _tenantService.CreateTenantAsync(tenantRequest);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TenantRequest tenantRequest)
    {
        var result = await _tenantService.UpdateTenantAsync(id, tenantRequest);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _tenantService.DeleteTenantAsync(id);
        return NoContent();
    }

    [HttpPost("search")]
    public async Task<ActionResult<PagingResponse<TenantResponse>>> SearchFormData([FromBody] TenantSearchRequest searchRequest)
    {
        var users = await _tenantService.SearchTenants(searchRequest);
        return Ok(users);
    }
}
