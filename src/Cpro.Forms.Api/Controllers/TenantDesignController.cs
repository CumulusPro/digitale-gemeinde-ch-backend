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

    [HttpGet("{tenantId}")]
    public async Task<ActionResult<TenantDesign>> Get([FromRoute] int tenantId)
    {
        var formTemplate = await _tenantDesignService.GetTenantDesign(tenantId);
        return Ok(formTemplate);
    }

    [HttpPost("{tenantId}")]
    public async Task<ActionResult<TenantDesign>> Post([FromBody] TenantDesign design, [FromRoute] int tenantId)
    {
        var form = await _tenantDesignService.CreateUpdateTenantDesign(design, tenantId);
        return Ok(form);
    }
}
