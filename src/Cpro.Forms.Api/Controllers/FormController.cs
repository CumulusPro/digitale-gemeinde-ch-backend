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

    [HttpGet("{formId}")]
    [HttpGet("{formId}/document/{documentId}")]
    [AllowAnonymous]
    public async Task<ActionResult<DocumentResponse>> GetFormData(string formId, [FromQuery] int? tenantId, string documentId = null)
    {
        var response = await _formService.GetFormDataAsync(formId, tenantId, documentId);
        return Ok(response);
    }

    [HttpPost("Submit")]
    [AllowAnonymous]
    public async Task<ActionResult<FormResponse>> SubmitForm([FromBody] dynamic jsonData, [FromHeader(Name = "Origin")] string origin)
    {
        FormResponse response = await _formService.SubmitTaskAsync(jsonData, origin);

        return Ok(response);
    }

    [HttpPut("{formId}/document/{documentId}")]
    public async Task<ActionResult<CreateFormDefinitionResponse>> UpdateFormData([FromBody] StatusRequest request,
        string formId, string documentId, [FromQuery] int? tenantId)
    {
        var response = await _formService.UpdateFormStatus(tenantId, formId, documentId, request.status);
        return Ok(response);
    }
    
    [HttpGet("count")]
    public async Task<ActionResult<List<FormNavigation>>> GetFormNavigation([FromQuery] int? tenantId)
    {
        var response = await _formService.GetFormNavigationAsync(tenantId);
        return Ok(response);
    }

    [HttpPost("search")]
    public async Task<ActionResult<PagingResponse<FormData>>> SearchFormData([FromBody] FormSearchRequest searchRequest, [FromQuery] int tenantId)
    {
        var form = await _formService.SearchFormData(searchRequest, tenantId);
        return Ok(form);
    }
}
