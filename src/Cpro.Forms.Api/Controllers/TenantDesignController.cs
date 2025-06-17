using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cpro.Forms.Api.Controllers;

[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class TenantDesignController : Controller
{
    private readonly ITenantDesignService _tenantDesignService;

    public TenantDesignController(ITenantDesignService tenantDesignService)
    {
        _tenantDesignService = tenantDesignService;
    }

    /// <summary>
    /// Retrieves a tenant design configuration by tenant ID.
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>Tenant design configuration</returns>
    [HttpGet("{tenantId}")]
    public async Task<ActionResult<TenantDesign>> Get([FromRoute] int tenantId)
    {
        var formTemplate = await _tenantDesignService.GetTenantDesign(tenantId);
        return Ok(formTemplate);
    }

    /// <summary>
    /// Creates or updates a tenant design configuration.
    /// </summary>
    /// <param name="design">The tenant design configuration to save</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The saved tenant design configuration</returns>
    [HttpPost("{tenantId}")]
    public async Task<ActionResult<TenantDesign>> Post([FromBody] TenantDesign design, [FromRoute] int tenantId)
    {
        var form = await _tenantDesignService.CreateUpdateTenantDesign(design, tenantId);
        return Ok(form);
    }
}
