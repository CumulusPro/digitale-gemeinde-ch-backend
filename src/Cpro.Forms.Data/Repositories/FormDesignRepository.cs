using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;


public class FormDesignRepository : RepositoryBase<FormDesign, SqlContext>, IFormDesignRepository
{
    public FormDesignRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    public async Task<FormDesign> CreateFormDesignAsync(FormDesign FormDesign)
    {
        FormDesign.DateCreated = DateTimeOffset.UtcNow;
        FormDesign.DateUpdated = DateTimeOffset.UtcNow;
        return await Insert(FormDesign);
    }

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

    public async Task<FormDesign?> GetFormDesignByFormId(string formId)
    {
        return await GetAllWithInclude(
            x => x.Id == formId,
            x => x.FormStates,
            x => x.Designers,
            x => x.Processors,
            x => x.Tags)
        .FirstOrDefaultAsync();        
    }

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

    public async Task<int> GetFormDesignCountAsync()
    {
        var query = _context.FormDesigns.AsQueryable();
        return await query.CountAsync();
    }

    public async Task<FormDesign> DeleteFormDesignAsync(string formId, int tenantId)
    {
        var FormDesign = await GetFormDesign(formId, tenantId);
        if (FormDesign != null)
        {
            return await Delete(FormDesign);
        }

        return null;
    }

    public async Task<PagingResponse<FormDesign>> SearchFormDesignsAsync(SearchRequest searchParameters, int tenantId)
    {
        var query = _context.FormDesigns
            .Include(f => f.Tags)
            .ThenInclude(fdt => fdt.Tag)
            .Where(x => x.Id != null && x.TenantId == tenantId)
            .AsQueryable();

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

    public async Task<FormDesign> UpdateFormDesignAsync(string formId, FormDesign formDesign)
    {
        formDesign.DateUpdated = DateTimeOffset.UtcNow;
        return await Update(formDesign);
    }

    public async Task<List<Tag>> GetTagsByNamesAsync(List<string> names)
    {
        return await _context.Tags
        .Where(t => names.Contains(t.TagName))
        .ToListAsync();
    }

    public async Task AddTagsAsync(List<Tag> tags)
    {
        if (tags != null && tags.Any())
        {
            await _context.Tags.AddRangeAsync(tags);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<string>> GetAllDistinctTagNamesAsync()
    {
        return await _context.Tags
            .OrderBy(t => t.TagName)
            .Select(t => t.TagName)
            .ToListAsync();
    }
}
