using Cpro.Forms.Data.Models.User;
using Peritos.Common.Abstractions.Paging;
using Peritos.Common.Data;

namespace Cpro.Forms.Data.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<List<User>> GetUsersByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User> CreateUserAsync(User user);
    Task<PagingResponse<User>> SearchUsers(UserSearchRequest searchRequest);
    Task<User> UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
    Task<User?> GetUserByEmailAndTenantAsync(string email, int tenantId);
}