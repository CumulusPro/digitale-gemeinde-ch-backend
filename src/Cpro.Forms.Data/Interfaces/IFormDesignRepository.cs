using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;

public interface IFormDesignRepository : IRepository<FormDesign>
{
    public Task<FormDesign> CreateFormDesignAsync(FormDesign FormDesign);
    public Task<FormDesign?> GetFormDesign(string formId, int tenantId);
    Task<FormDesign?> GetFormDesignByFormId(string formId);
    public Task<List<FormDesign>> GetFormDesignsByTenantId(int tenantId);
    public Task<int> GetFormDesignCountAsync();
    public Task<FormDesign> DeleteFormDesignAsync(string formId, int tenantId);
    public Task<PagingResponse<FormDesign>> SearchFormDesignsAsync(SearchRequest searchParameters, int tenantId);
    public Task<FormDesign> UpdateFormDesignAsync(string formId, FormDesign formDesign);
}
