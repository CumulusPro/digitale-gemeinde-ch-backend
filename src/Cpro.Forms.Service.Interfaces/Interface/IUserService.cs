using Cpro.Forms.Service.Models.Enums;
using Cpro.Forms.Service.Models.User;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Services;

public interface IUserService
{
    Task<UserResponse?> GetUserByIdAsync(Guid id);
    Task<UserResponse> CreateUserAsync(UserRequest user);
    Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest user);
    Task DeleteUserAsync(Guid id);
    Task<UserResponse> AssignUserRoleAsync(Guid id, Role newRole, string loggedUserEmail);
    Task<PagingResponse<UserResponse>> SearchUsers(UserSearchRequest searchRequest);
}