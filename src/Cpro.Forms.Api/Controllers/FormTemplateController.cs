using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Api.Controllers;

[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class FormTemplateController : Controller
{
    private readonly IFormTemplateService _formTemplateService;

    public FormTemplateController(IFormTemplateService formTemplateService)
    {
        _formTemplateService = formTemplateService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FormTemplate>> GetFormTemplate([FromRoute]string id, [FromQuery] int tenantId)
    {
        var formTemplate = await _formTemplateService.GetFormTemplate(id);
        return Ok(formTemplate);
    }

    [HttpPost]
    public async Task<ActionResult<FormTemplate>> CreateFormTemplate([FromBody] CreateFormTemplateRequest formTemplate, [FromQuery] int tenantId)
    {
        var form = await _formTemplateService.CreateFormTemplate(formTemplate, tenantId);
        return Ok(form);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FormTemplate>> UpdateFormTemplate([FromBody] FormTemplate formTemplate, string id, [FromQuery] int tenantId)
    {
        var form = await _formTemplateService.UpdateFormTemplate(id, formTemplate, tenantId);
        return Ok(form);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFormTemplate(string id, [FromQuery] int tenantId)
    {
        await _formTemplateService.GetFormTemplate(id);
        return NoContent();
    }

    [HttpPost("search")]
    public async Task<ActionResult<PagingResponse<FormTemplate>>> SearchFormTemplate([FromBody] SearchRequest searchRequest, [FromQuery] int tenantId)
    {
        var form = await _formTemplateService.SearchFormTemplate(searchRequest);
        return Ok(form);
    }
}
