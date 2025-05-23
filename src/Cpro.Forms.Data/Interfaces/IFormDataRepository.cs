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
