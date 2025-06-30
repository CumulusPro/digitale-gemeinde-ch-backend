using Cpro.Forms.Service.Models;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Services;

public interface IFormService
{
    public Task<DocumentResponse> GetFormDataAsync(string formId, int? tenantId, string documentId);
    public Task<List<FormNavigation>> GetFormNavigationAsync(int? tenantId, string email);
    public Task<PagingResponse<FormData>> SearchFormData(FormSearchRequest searchRequest, int tenantId);
    public Task<FormResponse> SubmitTaskAsync(dynamic jsonData, string origin);
    public Task<DocumentResponse> UpdateFormStatus(int? tenantId, string formId, string documentId, string status);
}
