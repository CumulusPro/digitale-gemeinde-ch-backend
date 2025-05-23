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

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await Get(x => x.Email == email);
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
