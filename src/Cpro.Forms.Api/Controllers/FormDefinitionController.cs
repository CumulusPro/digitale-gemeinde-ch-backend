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

    /// <summary>
    /// Retrieves a form definition by its ID and tenant.
    /// </summary>
    /// <param name="formId">The unique identifier of the form</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>Form definition response</returns>
    [HttpGet("{formId}")]
    [AllowAnonymous]
    public async Task<ActionResult<DocumentResponse>> GetFormDefinition(string formId, [FromQuery] int? tenantId)
    {
        var response = await _formService.GetFormDefinitionResponseAsync(formId, tenantId);
        return Ok(response);
    }

    /// <summary>
    /// Creates a new form definition with the specified field configuration.
    /// </summary>
    /// <param name="fieldRequest">The field request containing form configuration</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>Created form definition response</returns>
    [HttpPost("")]
    public async Task<ActionResult<CreateFormDefinitionResponse>> CreateFormDefinition([FromBody] FieldRequest fieldRequest, 
        [FromQuery] int? tenantId)
    {
        var response = await _formService.CreateFormDefinitionAsync(fieldRequest, string.Empty, tenantId, _requestContext.UserEmail);
        return Ok(response);
    }

    /// <summary>
    /// Updates an existing form definition with new field configuration.
    /// </summary>
    /// <param name="fieldRequest">The field request containing updated form configuration</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="formId">The unique identifier of the form to update</param>
    /// <returns>Updated form definition response</returns>
    [HttpPut("{formId}")]
    public async Task<ActionResult<CreateFormDefinitionResponse>> UpdateFormDefinition([FromBody] FieldRequest fieldRequest, [FromQuery] int? tenantId,
        string formId = "")
    {
        var response = await _formService.CreateFormDefinitionAsync(fieldRequest, formId, tenantId, _requestContext.UserEmail);
        return Ok(response);
    }

    /// <summary>
    /// Activates or deactivates a form definition.
    /// </summary>
    /// <param name="formId">The unique identifier of the form</param>
    /// <param name="isActive">Whether the form should be active</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>NoContent response</returns>
    [HttpPut("{formId}/activate/{isActive}")]
    public async Task<ActionResult> ActivateFormDefinition(string formId, bool isActive, [FromQuery] int? tenantId)
    {
        await _formService.ActivateFormDefinitionAsync(formId, isActive, tenantId);
        return NoContent();
    }

    /// <summary>
    /// Deletes a form definition and its associated storage.
    /// </summary>
    /// <param name="id">The unique identifier of the form to delete</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>NoContent response</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, [FromQuery] int tenantId)
    {
        await _formService.DeleteFormDesignAsync(id, tenantId);
        return NoContent();
    }

    /// <summary>
    /// Searches for form designs based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>Paged response containing matching form designs</returns>
    [HttpPost("search")]
    public async Task<ActionResult<PagingResponse<FormDesign>>> SearchFormDesigns([FromBody] SearchRequest searchRequest, [FromQuery] int tenantId)
    {
        var form = await _formService.SearchFormDesignsAsync(searchRequest, tenantId);
        return Ok(form);
    }

    /// <summary>
    /// Creates a duplicate of an existing form definition with a new ID and timestamped name.
    /// </summary>
    /// <param name="formId">The unique identifier of the form to duplicate</param>
    /// <returns>Duplicated form definition response</returns>
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

    /// <summary>
    /// Exports a form definition as a signed URL for JSON download.
    /// </summary>
    /// <param name="formId">The unique identifier of the form to export</param>
    /// <returns>Signed URL for form JSON download</returns>
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

    /// <summary>
    /// Imports a form definition from a JSON file, either creating a new form or updating an existing one.
    /// </summary>
    /// <param name="request">The import request containing the file and optional existing form ID</param>
    /// <returns>Created or updated form definition response</returns>
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

    /// <summary>
    /// Retrieves all versions of a form definition.
    /// </summary>
    /// <param name="formId">The unique identifier of the form</param>
    /// <returns>List of form design versions</returns>
    [HttpPost("{formId}/versions")]
    public async Task<IActionResult> GetAllVersions(string formId)
    {
        var result = await _formHistoryService.GetAllVersions(formId);
        return Ok(result);
    }

    /// <summary>
    /// Restores a specific version of a form definition.
    /// </summary>
    /// <param name="formId">The unique identifier of the form</param>
    /// <param name="version">The version number to restore</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>Restored form definition response</returns>
    [HttpPost("{formId}/versions/{version}/restore")]
    public async Task<IActionResult> RestoreVersion(string formId, int version, [FromQuery] int? tenantId)
    {
        var fieldRequest = await _formHistoryService.GetVersion(formId, version);
        var result = await _formService.CreateFormDefinitionAsync(fieldRequest, formId, tenantId, _requestContext.UserEmail);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all distinct tag names used across form designs.
    /// </summary>
    /// <returns>List of unique tag names</returns>
    [HttpGet("get-distinct-tags")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllDistinctTags()
    {
        var response = await _formService.GetAllDistinctTagNamesAsync();
        return Ok(response);
    }
}
