using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;

public interface IFormTemplateRepository : IRepository<FormTemplate>
{
    public Task<FormTemplate> CreateFormTemplateAsync(FormTemplate formTemplate);
    public Task<FormTemplate> DeleteFormTemplateAsync(string formTemplateId);
    public Task<FormTemplate> GetFormTemplate(string formTemplateId);
    public Task<PagingResponse<FormTemplate>> SearchFormTemplatesAsync(SearchRequest searchParameters);
    public Task<FormTemplate> UpdateFormTemplateAsync(string formTemplateId, FormTemplate formTemplate);
}