using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models.User;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;
using Peritos.Common.Data.Extensions;

namespace Cpro.Forms.Data.Repositories;

/// <summary>
/// Repository for managing user operations including creation, updates, deletion, and retrieval.
/// </summary>
public class UserRepository : RepositoryBase<User, SqlContext>, IUserRepository
{
    public UserRepository(SqlContext context, IRequestContext requestContext = null) : base(context, requestContext)
    {
    }

    /// <summary>
    /// Creates a new user in the database.
    /// </summary>
    /// <param name="user">The user to create</param>
    /// <returns>The created user</returns>
    public async Task<User> CreateUserAsync(User user)
    {
        return await Insert(user);
    }

    /// <summary>
    /// Deletes a user from the database.
    /// </summary>
    /// <param name="user">The user to delete</param>
    public async Task DeleteUserAsync(User user)
    {
        await Delete(user);
    }

    /// <summary>
    /// Retrieves all users with a specific email address.
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <returns>A list of users with the specified email</returns>
    public async Task<List<User>> GetUsersByEmailAsync(string email)
    {
        return await GetAll(x => x.Email == email)?.ToListAsync() ?? new List<User>();
    }

    /// <summary>
    /// Retrieves a user by email address and tenant ID.
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The user if found; otherwise null</returns>
    public async Task<User?> GetUserByEmailAndTenantAsync(string email, int tenantId)
    {
        return await Get(u => u.Email == email && u.TenantId == tenantId);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <returns>The user if found; otherwise null</returns>
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await Get(x => x.Id == id);
    }

    /// <summary>
    /// Searches for users based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria including email, first name, tenant ID, and role</param>
    /// <returns>A paged response containing matching users</returns>
    public async Task<PagingResponse<User>> SearchUsers(UserSearchRequest searchRequest)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchRequest.Email))
        {
            query = query.Where(u => u.Email.Contains(searchRequest.Email));
        }

        if (!string.IsNullOrWhiteSpace(searchRequest.FirstName))
        {
            query = query.Where(u => u.FirstName != null && u.FirstName.Contains(searchRequest.FirstName));
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

    /// <summary>
    /// Updates an existing user in the database.
    /// </summary>
    /// <param name="user">The updated user information</param>
    /// <returns>The updated user</returns>
    public async Task<User> UpdateUserAsync(User user)
    {
        return await Update(user);
    }
}
