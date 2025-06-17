using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Data;

namespace Cpro.Forms.Data.Repositories;

/// <summary>
/// Repository for managing form design history operations including version retrieval and storage.
/// </summary>
public class FormDesignHistoryRepository : RepositoryBase<FormDesignHistory, SqlContext>, IFormDesignHistoryRepository
{
    public FormDesignHistoryRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    /// <summary>
    /// Retrieves all versions of a form design ordered by version number in descending order.
    /// </summary>
    /// <param name="formId">The unique identifier of the form design</param>
    /// <returns>A list of form design history records with related entities</returns>
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

    /// <summary>
    /// Retrieves a specific version of a form design by form ID and version number.
    /// </summary>
    /// <param name="formId">The unique identifier of the form design</param>
    /// <param name="version">The version number to retrieve</param>
    /// <returns>The form design history record if found; otherwise null</returns>
    public async Task<FormDesignHistory?> GetVersion(string formId, int version)
    {
        return await GetAllWithInclude(
            x => x.FormDesignId == formId && x.Version == version,
            x => x.FormStates,
            x => x.Designers,
            x => x.Processors)
        .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Saves a form design history record with creation and update timestamps.
    /// </summary>
    /// <param name="formDesign">The form design history record to save</param>
    /// <returns>The saved form design history record</returns>
    public async Task<FormDesignHistory> SaveFormDesignHistoryAsync(FormDesignHistory formDesign)
    {
        formDesign.DateCreated = DateTimeOffset.UtcNow;
        formDesign.DateUpdated = DateTimeOffset.UtcNow;
        return await Insert(formDesign);
    }
}
