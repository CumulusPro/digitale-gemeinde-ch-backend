using Cpro.Forms.Service.Models;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Services;

public interface IFormTemplateService
{
    public Task<FormTemplate> CreateFormTemplate(CreateFormTemplateRequest formTemplate, int? tenantId);
    public Task DeleteFormTemplate(string formTemplateId);
    public Task<FormTemplate> GetFormTemplate(string formTemplateId);
    public Task<PagingResponse<FormTemplate>> SearchFormTemplate(SearchRequest searchRequest);
    public Task<FormTemplate> UpdateFormTemplate(string formTemplateId, FormTemplate formTemplate, int tenantId);
}
