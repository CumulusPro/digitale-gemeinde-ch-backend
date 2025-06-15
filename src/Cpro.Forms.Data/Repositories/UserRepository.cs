using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models.User;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;

public class UserRepository : RepositoryBase<User, SqlContext>, IUserRepository
{
    public UserRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    public async Task<User> CreateUserAsync(User user)
    {
        return await Insert(user);
    }

    public async Task DeleteUserAsync(User user)
    {
        await Delete(user);
    }

    public async Task<List<User>> GetUsersByEmailAsync(string email)
    {
        return await GetAll(x => x.Email == email)?.ToListAsync() ?? new List<User>();
    }

    public async Task<User?> GetUserByEmailAndTenantAsync(string email, int tenantId)
    {
        return await Get(u => u.Email == email && u.TenantId == tenantId);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await Get(x => x.Id == id);
    }

    public async Task<PagingResponse<User>> SearchUsers(UserSearchRequest searchRequest)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchRequest.Email))
        {
            query = query.Where(u => u.Email.Contains(searchRequest.Email));
        }

        if (!string.IsNullOrWhiteSpace(searchRequest.FirstName))
        {
            query = query.Where(u => u.Email.Contains(searchRequest.FirstName));
        }

        if (searchRequest.TenantId.HasValue)
        {
            query = query.Where(u => u.TenantId == searchRequest.TenantId.Value);
        }
        if (searchRequest.Role.HasValue)
        {
            query = query.Where(u => u.Role == searchRequest.Role.Value);
        }

        var totalItemCount = await query.CountAsync();
        var searchResults = await query.ToPagedList(searchRequest);

        return new PagingResponse<User>
        {
            Data = searchResults,
            TotalCount = totalItemCount,
            PageNumber = searchRequest.Page,
            PageSize = searchRequest.PageSize,
        };
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        return await Update(user);
    }
}
