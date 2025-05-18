using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Data;

namespace Cpro.Forms.Data.Repositories;

public interface IFormDesignHistoryRepository : IRepository<FormDesignHistory>
{
    public Task<FormDesignHistory> SaveFormDesignHistoryAsync(FormDesignHistory formDesign);
    public Task<List<FormDesignHistory>> GetAllVersions(string formId);
    public Task<FormDesignHistory?> GetVersion(string formId, int version);
}

public class FormDesignHistoryRepository : RepositoryBase<FormDesignHistory, SqlContext>, IFormDesignHistoryRepository
{
    public FormDesignHistoryRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    public async Task<List<FormDesignHistory>> GetAllVersions(string formId)
    {
        return await GetAllWithInclude(
            x => x.FormDesignId == formId,
            x => x.FormStates,
            x => x.Designers,
            x => x.Processors)
            .OrderByDescending(x => x.Version)
        .ToListAsync();
    }

    public async Task<FormDesignHistory?> GetVersion(string formId, int version)
    {
        return await GetAllWithInclude(
            x => x.FormDesignId == formId && x.Version == version,
            x => x.FormStates,
            x => x.Designers,
            x => x.Processors)
        .FirstOrDefaultAsync();
    }

    public async Task<FormDesignHistory> SaveFormDesignHistoryAsync(FormDesignHistory formDesign)
    {
        formDesign.DateCreated = DateTimeOffset.UtcNow;
        formDesign.DateUpdated = DateTimeOffset.UtcNow;
        return await Insert(formDesign);
    }
}
