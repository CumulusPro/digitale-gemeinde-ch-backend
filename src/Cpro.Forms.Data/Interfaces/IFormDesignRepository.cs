using Cpro.Forms.Data.Models;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;

namespace Cpro.Forms.Data.Repositories;

public interface IFormDesignRepository : IRepository<FormDesign>
{
    public Task<FormDesign> CreateFormDesignAsync(FormDesign FormDesign);
    public Task<FormDesign?> GetFormDesign(string formId, int tenantId);
    Task<FormDesign?> GetFormDesignByFormId(string formId);
    public Task<List<FormDesign>> GetFormDesignsByTenantId(int tenantId, string email);
    public Task<int> GetFormDesignCountAsync();
    public Task<FormDesign> DeleteFormDesignAsync(string formId, int tenantId);
    public Task<PagingResponse<FormDesign>> SearchFormDesignsAsync(SearchRequest searchParameters, int tenantId, string email);
    public Task<FormDesign> UpdateFormDesignAsync(string formId, FormDesign formDesign);
    Task<List<Tag>> GetTagsByNamesAsync(List<string> names);
    Task AddTagsAsync(List<Tag> tags);
    Task<List<string>> GetAllDistinctTagNamesAsync();
    Task RemoveUserFromFormDesigns(string email, int tenantId);
}
