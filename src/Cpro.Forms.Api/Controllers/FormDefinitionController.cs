using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Peritos.Common.Abstractions;
using Peritos.Common.Api.Paging;

namespace Cpro.Forms.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class FormDefinitionController : Controller
{
    private readonly IFormDesignerService _formService;
    private readonly IFormDesignerHistoryService _formHistoryService;
    private readonly IRequestContext _requestContext;

    public FormDefinitionController(IFormDesignerService formService, 
        IFormDesignerHistoryService formHistoryService, 
        IRequestContext requestContext)
    {
        _formService = formService;
        _formHistoryService = formHistoryService;
        _requestContext = requestContext;
    }

    [HttpGet("{formId}")]
    [AllowAnonymous]
    public async Task<ActionResult<DocumentResponse>> GetFormDefinition(string formId, [FromQuery] int? tenantId)
    {
        var response = await _formService.GetFormDefinitionResponseAsync(formId, tenantId);
        return Ok(response);
    }

    [HttpPost("")]
    public async Task<ActionResult<CreateFormDefinitionResponse>> CreateFormDefinition([FromBody] FieldRequest fieldRequest, 
        [FromQuery] int? tenantId)
    {
        var response = await _formService.CreateFormDefinitionAsync(fieldRequest, string.Empty, tenantId, _requestContext.UserEmail);
        return Ok(response);
    }

    [HttpPut("{formId}")]
    public async Task<ActionResult<CreateFormDefinitionResponse>> UpdateFormDefinition([FromBody] FieldRequest fieldRequest, [FromQuery] int? tenantId,
        string formId = "")
    {
        var response = await _formService.CreateFormDefinitionAsync(fieldRequest, formId, tenantId, _requestContext.UserEmail);
        return Ok(response);
    }

    [HttpPut("{formId}/activate/{isActive}")]
    public async Task<ActionResult> ActivateFormDefinition(string formId, bool isActive, [FromQuery] int? tenantId)
    {
        await _formService.ActivateFormDefinitionAsync(formId, isActive, tenantId);
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, [FromQuery] int tenantId)
    {
        await _formService.DeleteFormDesignAsync(id, tenantId);
        return NoContent();
    }

    [HttpPost("search")]
    public async Task<ActionResult<PagingResponse<FormDesign>>> SearchFormDesigns([FromBody] SearchRequest searchRequest, [FromQuery] int tenantId)
    {
        var form = await _formService.SearchFormDesignsAsync(searchRequest, tenantId);
        return Ok(form);
    }

    [HttpPost("{formId}/duplicate")]
    public async Task<ActionResult<CreateFormDefinitionResponse>> DuplicateFormDefinition(string formId)
    {
        if (string.IsNullOrWhiteSpace(formId))
        {
            return BadRequest("FormId and TenantId are required.");
        }

        var response = await _formService.DuplicateFormDefinitionAsync(formId, _requestContext.UserEmail);
        return Ok(response);
    }

    [HttpPost("{formId}/export")]
    public async Task<IActionResult> ExportForm(string formId)
    {
        if (string.IsNullOrWhiteSpace(formId))
        {
            return BadRequest("FormId is required.");
        }

        var signedUrl = await _formService.GetFormDataJsonAsync(formId);
        return Ok(signedUrl);
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportForm([FromForm] ImportFormRequest request)
    {
        using var stream = new StreamReader(request.File.OpenReadStream());
        var jsonContent = await stream.ReadToEndAsync();
        var fieldRequest = JsonConvert.DeserializeObject<FieldRequest>(jsonContent);

        // If existingFormId is passed, update the form. Else, create a new one.
        var form = await _formService.CreateFormDefinitionAsync(fieldRequest, request.ExistingFormId, request.tenantId, _requestContext.UserEmail);

        return Ok(form);
    }

    [HttpPost("{formId}/versions")]
    public async Task<IActionResult> GetAllVersions(string formId)
    {
        var result = await _formHistoryService.GetAllVersions(formId);
        return Ok(result);
    }

    [HttpPost("{formId}/versions/{version}/restore")]
    public async Task<IActionResult> RestoreVersion(string formId, int version, [FromQuery] int? tenantId)
    {
        var fieldRequest = await _formHistoryService.GetVersion(formId, version);
        var result = await _formService.CreateFormDefinitionAsync(fieldRequest, formId, tenantId, _requestContext.UserEmail);

        return Ok(result);
    }

    [HttpGet("get-distinct-tags")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllDistinctTags()
    {
        var response = await _formService.GetAllDistinctTagNamesAsync();
        return Ok(response);
    }
}
