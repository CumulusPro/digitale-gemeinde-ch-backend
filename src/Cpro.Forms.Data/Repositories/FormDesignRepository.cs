using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Cpro.Forms.Data.Models.User;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;

/// <summary>
/// Repository for managing form design operations including creation, updates, deletion, and retrieval with related entities.
/// </summary>
public class FormDesignRepository : RepositoryBase<FormDesign, SqlContext>, IFormDesignRepository
{
    public FormDesignRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    /// <summary>
    /// Creates a new form design with creation and update timestamps.
    /// </summary>
    /// <param name="FormDesign">The form design to create</param>
    /// <returns>The created form design</returns>
    public async Task<FormDesign> CreateFormDesignAsync(FormDesign FormDesign)
    {
        FormDesign.DateCreated = DateTimeOffset.UtcNow;
        FormDesign.DateUpdated = DateTimeOffset.UtcNow;
        return await Insert(FormDesign);
    }

    /// <summary>
    /// Retrieves a form design by its ID and tenant ID, including related entities.
    /// </summary>
    /// <param name="formId">The unique identifier of the form design</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The form design with related entities if found; otherwise null</returns>
    public async Task<FormDesign?> GetFormDesign(string formId, int tenantId)
    {
        if (tenantId > 0)
        {
            return await GetAllWithInclude(
                x => x.Id == formId && x.TenantId == tenantId,
                x => x.FormStates,
                x => x.Designers,
                x => x.Processors,
                x => x.Tags)
            .FirstOrDefaultAsync();
        }
        return await GetAllWithInclude(
            x => x.Id == formId,
            x => x.FormStates,
            x => x.Designers,
            x => x.Processors,
            x => x.Tags)
        .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves a form design by its ID, including related entities.
    /// </summary>
    /// <param name="formId">The unique identifier of the form design</param>
    /// <returns>The form design with related entities if found; otherwise null</returns>
    public async Task<FormDesign?> GetFormDesignByFormId(string formId)
    {
        return await _context.FormDesigns
            .Where(x => x.Id == formId)
            .Include(x => x.FormStates)
            .Include(x => x.Designers)
            .Include(x => x.Processors)
            .Include(x => x.Tags).ThenInclude(tag => tag.Tag)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves all form designs for a specific tenant, including related entities.
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>A list of form designs with related entities for the tenant</returns>
    public async Task<List<FormDesign>> GetFormDesignsByTenantId(int tenantId)
    {
        return await GetAllWithInclude(
            x => x.Id != null && x.TenantId == tenantId,
            x => x.FormStates,
            x => x.Designers,
            x => x.Processors,
            x => x.Tags)
        .ToListAsync();
    }

    /// <summary>
    /// Gets the total count of form designs in the system.
    /// </summary>
    /// <returns>The total count of form designs</returns>
    public async Task<int> GetFormDesignCountAsync()
    {
        var query = _context.FormDesigns.AsQueryable();
        return await query.CountAsync();
    }

    /// <summary>
    /// Deletes a form design by its ID and tenant ID.
    /// </summary>
    /// <param name="formId">The unique identifier of the form design</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The deleted form design if found; otherwise null</returns>
    public async Task<FormDesign> DeleteFormDesignAsync(string formId, int tenantId)
    {
        var FormDesign = await GetFormDesign(formId, tenantId);
        if (FormDesign != null)
        {
            return await Delete(FormDesign);
        }

        return null;
    }

    /// <summary>
    /// Searches for form designs based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchParameters">The search criteria including keywords and ordering</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="email">logged in user's email</param>
    /// <returns>A paged response containing matching form designs</returns>
    public async Task<PagingResponse<FormDesign>> SearchFormDesignsAsync(SearchRequest searchParameters, int tenantId, string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId)
            ?? throw new KeyNotFoundException($"User not found with email: {email}");

        var query = _context.FormDesigns
            .Include(f => f.Tags).ThenInclude(fdt => fdt.Tag)
            .Include(f => f.Designers)
            .Include(f => f.Processors)
            .Where(x => x.Id != null && x.TenantId == tenantId)
            .AsQueryable();

        //Role - based filtering
        if (user.Role == Role.Designer)
        {
            query = query.Where(f => f.Designers.Any(d => d.Email == email));
        }
        else if (user.Role == Role.Processor)
        {
            query = query.Where(f => f.Processors.Any(p => p.Email == email));
        }
        // Admins see everything, no filter needed

        if (!string.IsNullOrWhiteSpace(searchParameters.Keyword))
        {
            query = query.Where(t => t.Name.Contains(searchParameters.Keyword) || t.Tags.Any(t => t.Tag.TagName.Contains(searchParameters.Keyword)));
        }
        
        if (!string.IsNullOrWhiteSpace(searchParameters.OrderBy))
        {
            query = query.OrderBy(searchParameters.OrderBy, searchParameters.OrderByDirection == PagingParametersOrderByDirectionEnum.Descending);
        }
        else
        {
            query = query.OrderByDescending(t => t.DateUpdated);
        }

        var totalItemCount = await query.CountAsync();
        var searchResults = await query.ToPagedList(searchParameters);

        return new PagingResponse<FormDesign>
        { 
            Data= searchResults,
            TotalCount = totalItemCount,
            PageNumber = searchParameters.Page,
            PageSize = searchParameters.PageSize
        };
    }

    /// <summary>
    /// Updates an existing form design with an updated timestamp.
    /// </summary>
    /// <param name="formId">The unique identifier of the form design to update</param>
    /// <param name="formDesign">The updated form design</param>
    /// <returns>The updated form design</returns>
    public async Task<FormDesign> UpdateFormDesignAsync(string formId, FormDesign formDesign)
    {
        formDesign.DateUpdated = DateTimeOffset.UtcNow;
        return await Update(formDesign);
    }

    /// <summary>
    /// Retrieves tags by their names.
    /// </summary>
    /// <param name="names">A list of tag names to retrieve</param>
    /// <returns>A list of tags matching the provided names</returns>
    public async Task<List<Tag>> GetTagsByNamesAsync(List<string> names)
    {
        return await _context.Tags
        .Where(t => names.Contains(t.TagName))
        .ToListAsync();
    }

    /// <summary>
    /// Adds multiple tags to the database.
    /// </summary>
    /// <param name="tags">A list of tags to add</param>
    public async Task AddTagsAsync(List<Tag> tags)
    {
        if (tags != null && tags.Any())
        {
            await _context.Tags.AddRangeAsync(tags);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Retrieves all distinct tag names ordered alphabetically.
    /// </summary>
    /// <returns>A list of unique tag names</returns>
    public async Task<List<string>> GetAllDistinctTagNamesAsync()
    {
        return await _context.Tags
            .OrderBy(t => t.TagName)
            .Select(t => t.TagName)
            .ToListAsync();
    }

    /// <summary>
    /// Removes a user email from all form designs's designers and processors they are associated with.
    /// </summary>
    /// <param name="email">email of the user to be removed</param>
    /// <param name="tenantId">tenantId of the user to be removed</param>
    public async Task RemoveUserFromFormDesigns(string email, int tenantId)
    {
        var formDesigns = await _context.FormDesigns
            .Include(f => f.Designers)
            .Include(f => f.Processors)
            .Where(fd => fd.TenantId == tenantId && (fd.Designers.Any(d => d.Email == email) || fd.Processors.Any(p => p.Email == email)))
            .ToListAsync();

        if (formDesigns.Count == 0)
            return;

        foreach (var form in formDesigns)
        {
            bool modified = false;

            var newDesigners = form.Designers.Where(d => d.Email != email).ToList();
            if (newDesigners.Count != form.Designers.Count)
            {
                form.Designers = newDesigners;
                modified = true;
            }

            var newProcessors = form.Processors.Where(p => p.Email != email).ToList();
            if (newProcessors.Count != form.Processors.Count)
            {
                form.Processors = newProcessors;
                modified = true;
            }

            if (modified)
            {
                form.DateUpdated = DateTimeOffset.UtcNow;
                _context.FormDesigns.Update(form);
            }
        }

        await _context.SaveChangesAsync();
    }
}
