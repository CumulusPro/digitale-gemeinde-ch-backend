using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;


public class FormTemplateRepository : RepositoryBase<FormTemplate, SqlContext>, IFormTemplateRepository
{
    public FormTemplateRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    public async Task<FormTemplate> CreateFormTemplateAsync(FormTemplate formTemplate)
    {
        return await Insert(formTemplate);
    }

    public async Task<FormTemplate> DeleteFormTemplateAsync(string formTemplateId)
    {
        var formTemplate = await GetFormTemplate(formTemplateId);
        if (formTemplate != null)
        {
            return await Delete(formTemplate);
        }

        return null;
    }

    public async Task<FormTemplate> GetFormTemplate(string formTemplateId)
    {
        return await Get(x => x.Id == formTemplateId);
    }

    public async Task<PagingResponse<FormTemplate>> SearchFormTemplatesAsync(SearchRequest searchParameters)
    {
        var query = _context.FormTemplates.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchParameters.Keyword))
        {
            query = query.Where(t => t.Name.Contains(searchParameters.Keyword));
        }

        var totalItemCount = await query.CountAsync();
        var searchResults = await query.ToPagedList(searchParameters);

        return new PagingResponse<FormTemplate> 
        { 
            Data = searchResults,
            TotalCount = totalItemCount,
            PageNumber = searchParameters.Page,
            PageSize = searchParameters.PageSize
        };
    }

    public async Task<FormTemplate> UpdateFormTemplateAsync(string formTemplateId, FormTemplate formTemplate)
    {
        // Assuming Id is a property used to find the specific form template
        var existingTemplate = await GetFormTemplate(formTemplateId);
        if (existingTemplate != null)
        {
            return await Update(formTemplate);
        }

        return null;
    }
}
