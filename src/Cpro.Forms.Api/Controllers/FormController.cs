using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class FormController : Controller
{
    private readonly IFormService _formService;

    public FormController(IFormService formService)
    {
        _formService = formService;
    }

    /// <summary>
    /// Retrieves form data based on the provided parameters.
    /// </summary>
    /// <param name="formId">The unique identifier of the form</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="documentId">Optional document identifier for existing form data</param>
    /// <returns>Form data response</returns>
    [HttpGet("{formId}")]
    [HttpGet("{formId}/document/{documentId}")]
    [AllowAnonymous]
    public async Task<ActionResult<DocumentResponse>> GetFormData(string formId, [FromQuery] int? tenantId, string documentId = null)
    {
        var response = await _formService.GetFormDataAsync(formId, tenantId, documentId);
        return Ok(response);
    }

    /// <summary>
    /// Submits form data
    /// </summary>
    /// <param name="jsonData">The form data to submit</param>
    /// <param name="origin">The origin URL for payment processing</param>
    /// <returns>Form submission response with document ID</returns>
    [HttpPost("Submit")]
    [AllowAnonymous]
    public async Task<ActionResult<FormResponse>> SubmitForm([FromBody] dynamic jsonData, [FromHeader(Name = "Origin")] string origin)
    {
        FormResponse response = await _formService.SubmitTaskAsync(jsonData, origin);

        return Ok(response);
    }

    /// <summary>
    /// Updates the status of an existing form.
    /// </summary>
    /// <param name="request">The status update request</param>
    /// <param name="formId">The form identifier</param>
    /// <param name="documentId">The document identifier</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>Updated form definition response</returns>
    [HttpPut("{formId}/document/{documentId}")]
    public async Task<ActionResult<CreateFormDefinitionResponse>> UpdateFormData([FromBody] StatusRequest request,
        string formId, string documentId, [FromQuery] int? tenantId)
    {
        var response = await _formService.UpdateFormStatus(tenantId, formId, documentId, request.status);
        return Ok(response);
    }
    
    /// <summary>
    /// Retrieves form navigation data
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>List of form navigation items with status-based counts</returns>
    [HttpGet("count")]
    public async Task<ActionResult<List<FormNavigation>>> GetFormNavigation([FromQuery] int? tenantId)
    {
        var response = await _formService.GetFormNavigationAsync(tenantId);
        return Ok(response);
    }

    /// <summary>
    /// Searches form data based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>Paged response containing matching form data</returns>
    [HttpPost("search")]
    public async Task<ActionResult<PagingResponse<FormData>>> SearchFormData([FromBody] FormSearchRequest searchRequest, [FromQuery] int tenantId)
    {
        var form = await _formService.SearchFormData(searchRequest, tenantId);
        return Ok(form);
    }
}
