using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;

/// <summary>
/// Repository for managing form data operations including creation, updates, deletion, and retrieval.
/// </summary>
public class FormDataRepository : RepositoryBase<FormData, SqlContext>, IFormDataRepository
{
    public FormDataRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    /// <summary>
    /// Creates a new form data record in the database.
    /// </summary>
    /// <param name="formData">The form data to create</param>
    /// <returns>The created form data</returns>
    public async Task<FormData> CreateFormDataAsync(FormData formData)
    {
        return await Insert(formData);
    }

    /// <summary>
    /// Deletes a form data record by its ID and tenant ID.
    /// </summary>
    /// <param name="formDataId">The unique identifier of the form data</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The deleted form data if found; otherwise null</returns>
    public async Task<FormData> DeleteFormDataAsync(string formDataId, int tenantId)
    {
        var formData = await GetFormData(formDataId);
        if (formData != null)
        {
            return await Delete(formData);
        }

        return null;
    }

    /// <summary>
    /// Retrieves a form data record by its unique identifier.
    /// </summary>
    /// <param name="formDataId">The unique identifier of the form data</param>
    /// <returns>The form data if found; otherwise null</returns>
    public async Task<FormData> GetFormData(string formDataId)
    {
        return await Get(x => x.Id == formDataId);
    }

    /// <summary>
    /// Gets the total count of form data records in the system.
    /// </summary>
    /// <returns>The total count of form data records</returns>
    public async Task<int> GetFormDataCountAsync()
    {
        var query = _context.FormDatas.AsQueryable();
        return await query.CountAsync();
    }

    /// <summary>
    /// Retrieves a form data record by its document identifier.
    /// </summary>
    /// <param name="documentId">The document identifier</param>
    /// <returns>The form data if found; otherwise null</returns>
    public async Task<FormData> GetFormDataByDocumentId(string documentId)
    {
        return await Get(x => x.DocumentId == documentId);
    }

    /// <summary>
    /// Retrieves all form data records for a specific tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>A list of form data records for the tenant</returns>
    public async Task<List<FormData>> GetFormDatasByTenantId(int tenantId)
    {
        return await GetAll(x => x.Id != null && x.TenantId == tenantId).ToListAsync();
    }

    /// <summary>
    /// Searches for form data records based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchParameters">The search criteria including keywords, form ID, and status</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>A paged response containing matching form data records</returns>
    public async Task<PagingResponse<FormData>> SearchFormDatasAsync(FormSearchRequest searchParameters, int tenantId)
    {
        var query = _context.FormDatas.Where(x => x.Id != null && x.TenantId == tenantId).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchParameters.Keyword))
        {
            query = query.Where(t => t.Name.Contains(searchParameters.Keyword));
        }

        if (!string.IsNullOrWhiteSpace(searchParameters.FormId))
        {
            query = query.Where(t => t.FormId.Equals(searchParameters.FormId));
            Console.WriteLine($"FormId: {searchParameters.FormId}");
        }

        if (!string.IsNullOrWhiteSpace(searchParameters.Status))
        {
            query = query.Where(t => t.Status.Equals(searchParameters.Status));
        }

        var totalItemCount = await query.CountAsync();
        var searchResults = await query.ToPagedList(searchParameters);

        return new PagingResponse<FormData> 
        { 
            Data= searchResults,
            TotalCount = totalItemCount,
            PageNumber = searchParameters.Page,
            PageSize = searchParameters.PageSize
        };
    }

    /// <summary>
    /// Updates an existing form data record.
    /// </summary>
    /// <param name="formDataId">The unique identifier of the form data to update</param>
    /// <param name="formData">The updated form data</param>
    /// <returns>The updated form data if found; otherwise null</returns>
    public async Task<FormData> UpdateFormDataAsync(string formDataId, FormData formData)
    {
        var existingTemplate = await GetFormData(formDataId);
        if (existingTemplate != null)
        {
            return await Update(formData);
        }

        return null;
    }

    /// <summary>
    /// Generates the next sequence number for document IDs using a database sequence.
    /// </summary>
    /// <returns>The next document ID sequence number as a string</returns>
    public async Task<string> GetNextSequenceDocumentId()
    {
        using var command = _context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT NEXT VALUE FOR dbo.DocumentIdSequence";
        command.CommandType = System.Data.CommandType.Text;

        await _context.Database.OpenConnectionAsync();

        var result = await command.ExecuteScalarAsync();
        return result.ToString();
    }
}
