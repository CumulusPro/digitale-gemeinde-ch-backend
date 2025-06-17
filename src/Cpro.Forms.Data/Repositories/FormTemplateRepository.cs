using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;

/// <summary>
/// Repository for managing form template operations including creation, updates, deletion, and retrieval.
/// </summary>
public class FormTemplateRepository : RepositoryBase<FormTemplate, SqlContext>, IFormTemplateRepository
{
    public FormTemplateRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    /// <summary>
    /// Creates a new form template in the database.
    /// </summary>
    /// <param name="formTemplate">The form template to create</param>
    /// <returns>The created form template</returns>
    public async Task<FormTemplate> CreateFormTemplateAsync(FormTemplate formTemplate)
    {
        return await Insert(formTemplate);
    }

    /// <summary>
    /// Deletes a form template by its unique identifier.
    /// </summary>
    /// <param name="formTemplateId">The unique identifier of the form template</param>
    /// <returns>The deleted form template if found; otherwise null</returns>
    public async Task<FormTemplate> DeleteFormTemplateAsync(string formTemplateId)
    {
        var formTemplate = await GetFormTemplate(formTemplateId);
        if (formTemplate != null)
        {
            return await Delete(formTemplate);
        }

        return null;
    }

    /// <summary>
    /// Retrieves a form template by its unique identifier.
    /// </summary>
    /// <param name="formTemplateId">The unique identifier of the form template</param>
    /// <returns>The form template if found; otherwise null</returns>
    public async Task<FormTemplate> GetFormTemplate(string formTemplateId)
    {
        return await Get(x => x.Id == formTemplateId);
    }

    /// <summary>
    /// Searches for form templates based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchParameters">The search criteria including keywords</param>
    /// <returns>A paged response containing matching form templates</returns>
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

    /// <summary>
    /// Updates an existing form template.
    /// </summary>
    /// <param name="formTemplateId">The unique identifier of the form template to update</param>
    /// <param name="formTemplate">The updated form template</param>
    /// <returns>The updated form template if found; otherwise null</returns>
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
