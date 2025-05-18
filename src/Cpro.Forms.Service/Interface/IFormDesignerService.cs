using Cpro.Forms.Service.Models;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Services;

public interface IFormDesignerService
{
    Task<FormDesign> CreateFormDefinitionAsync(FieldRequest fieldRequest, string formId, int? tenantId, string email);
    Task<string> GetFormDataJsonAsync(string formId);
    Task<DocumentResponse> GetFormDefinitionResponseAsync(string formId, int? tenantId);
    Task<List<FormDesign>> GetFormDesignsByTenantIdAsync(int tenantId);
    Task DeleteFormDesignAsync(string formId, int tenantId);
    Task<FormDesign> DuplicateFormDefinitionAsync(string formId, string email);
    Task<PagingResponse<FormDesign>> SearchFormDesignsAsync(SearchRequest searchRequest, int tenantId);
    Task ActivateFormDefinitionAsync(string formId, bool isActive, int? tenantId);
}
