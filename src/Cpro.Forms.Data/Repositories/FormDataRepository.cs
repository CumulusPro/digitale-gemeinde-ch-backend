using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;

public interface IFormDataRepository : IRepository<FormData>
{
    public Task<FormData> CreateFormDataAsync(FormData formData);
    public Task<FormData> DeleteFormDataAsync(string formDataId, int tenantId);
    public Task<FormData> GetFormData(string formDataId);
    public Task<int> GetFormDataCountAsync();
    public Task<FormData> GetFormDataByDocumentId(string documentId);
    public Task<List<FormData>> GetFormDatasByTenantId(int tenantId);
    public Task<PagingResponse<FormData>> SearchFormDatasAsync(FormSearchRequest searchParameters, int tenantId);
    public Task<FormData> UpdateFormDataAsync(string formDataId, FormData formData);
    public Task<string> GetNextSequenceDocumentId();
}

public class FormDataRepository : RepositoryBase<FormData, SqlContext>, IFormDataRepository
{
    public FormDataRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    public async Task<FormData> CreateFormDataAsync(FormData formData)
    {
        return await Insert(formData);
    }

    public async Task<FormData> DeleteFormDataAsync(string formDataId, int tenantId)
    {
        var formData = await GetFormData(formDataId);
        if (formData != null)
        {
            return await Delete(formData);
        }

        return null;
    }

    public async Task<FormData> GetFormData(string formDataId)
    {
        return await Get(x => x.Id == formDataId);
    }

    public async Task<int> GetFormDataCountAsync()
    {
        var query = _context.FormDatas.AsQueryable();
        return await query.CountAsync();
    }

    public async Task<FormData> GetFormDataByDocumentId(string documentId)
    {
        return await Get(x => x.DocumentId == documentId);
    }

    public async Task<List<FormData>> GetFormDatasByTenantId(int tenantId)
    {
        return await GetAll(x => x.Id != null && x.TenantId == tenantId).ToListAsync();
    }

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

    public async Task<FormData> UpdateFormDataAsync(string formDataId, FormData formData)
    {
        var existingTemplate = await GetFormData(formDataId);
        if (existingTemplate != null)
        {
            return await Update(formData);
        }

        return null;
    }

    public async Task<string> GetNextSequenceDocumentId()
    {
        var docId = await _context
                    .Set<NextValResult>()
                    .FromSqlRaw("SELECT NEXT VALUE FOR dbo.DocumentIdSequence AS Value")
                    .AsNoTracking()
                    .FirstAsync();

        return docId.Value.ToString();
    }
}
