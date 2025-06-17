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

    /// <summary>
    /// Retrieves a form template by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the form template</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>Form template information</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<FormTemplate>> GetFormTemplate([FromRoute]string id, [FromQuery] int tenantId)
    {
        var formTemplate = await _formTemplateService.GetFormTemplate(id);
        return Ok(formTemplate);
    }

    /// <summary>
    /// Creates a new form template.
    /// </summary>
    /// <param name="formTemplate">The form template creation request</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The created form template</returns>
    [HttpPost]
    public async Task<ActionResult<FormTemplate>> CreateFormTemplate([FromBody] CreateFormTemplateRequest formTemplate, [FromQuery] int tenantId)
    {
        var form = await _formTemplateService.CreateFormTemplate(formTemplate, tenantId);
        return Ok(form);
    }

    /// <summary>
    /// Updates an existing form template.
    /// </summary>
    /// <param name="formTemplate">The updated form template data</param>
    /// <param name="id">The unique identifier of the form template to update</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The updated form template</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<FormTemplate>> UpdateFormTemplate([FromBody] FormTemplate formTemplate, string id, [FromQuery] int tenantId)
    {
        var form = await _formTemplateService.UpdateFormTemplate(id, formTemplate, tenantId);
        return Ok(form);
    }

    /// <summary>
    /// Deletes a form template from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the form template to delete</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>NoContent response</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFormTemplate(string id, [FromQuery] int tenantId)
    {
        await _formTemplateService.GetFormTemplate(id);
        return NoContent();
    }

    /// <summary>
    /// Searches for form templates based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>Paged response containing matching form templates</returns>
    [HttpPost("search")]
    public async Task<ActionResult<PagingResponse<FormTemplate>>> SearchFormTemplate([FromBody] SearchRequest searchRequest, [FromQuery] int tenantId)
    {
        var form = await _formTemplateService.SearchFormTemplate(searchRequest);
        return Ok(form);
    }
}
