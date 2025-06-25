using AutoMapper;
using Cpro.Forms.Data.Models.Tenant;
using Cpro.Forms.Data.Models.User;
using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Models.User;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Services;

/// <summary>
/// Service for managing user operations including creation, updates, role assignment, and tenant management.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;
    private readonly IFormDesignerService _formDesignerService;

    public UserService(IUserRepository userRepository, IMapper mapper, ITenantRepository tenantRepository, IFormDesignerService formDesignerService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _tenantRepository = tenantRepository;
        _formDesignerService = formDesignerService;
    }

    /// <summary>
    /// Assigns a new role to a user. Only administrators can perform this operation.
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <param name="newRole">The new role to assign</param>
    /// <param name="loggedUserEmail">The email of the user performing the role assignment</param>
    /// <returns>The updated user response</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the user to be updated is not found</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the logged user is not an administrator</exception>
    public async Task<UserResponse> AssignUserRoleAsync(Guid id, Models.Enums.Role newRole, string loggedUserEmail)
    {
        var user = await _userRepository.GetUserByIdAsync(id)
            ?? throw new KeyNotFoundException($"User to be updated not found with id: {id}");

        var loggerUser = await _userRepository.GetUserByEmailAndTenantAsync(loggedUserEmail, user.TenantId);

        if (loggerUser?.Role != Role.Admin)
            throw new UnauthorizedAccessException("Only Admins can assign roles.");

        user.Role = _mapper.Map<Data.Models.User.Role>(newRole);
        var updated = await _userRepository.UpdateUserAsync(user);
        return _mapper.Map<UserResponse>(updated);
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="user">The user request containing user details</param>
    /// <returns>The created user response</returns>
    public async Task<UserResponse> CreateUserAsync(UserRequest user)
    {
        var dbUser = _mapper.Map<User>(user);
        dbUser.Id = Guid.NewGuid();

        var saved = await _userRepository.CreateUserAsync(dbUser);
        return _mapper.Map<UserResponse>(saved);
    }

    /// <summary>
    /// Deletes a user from the system.
    /// Also removes the user from any associated form designs.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete</param>
    /// <exception cref="KeyNotFoundException">Thrown when the user is not found</exception>
    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(id)
            ?? throw new KeyNotFoundException($"User not found with id: {id}");

        await _userRepository.DeleteUserAsync(user);

        // Remove user from all Designer and Processor entries
        await _formDesignerService.RemoveUserFromFormDesigns(user.Email, user.TenantId);
    }

    /// <summary>
    /// Retrieves a user by email and tenant ID.
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The user response if found; otherwise null</returns>
    public async Task<UserResponse?> GetUserByEmailAndTenantAsync(string email, int tenantId)
    {
        var user = await _userRepository.GetUserByEmailAndTenantAsync(email, tenantId);
        return _mapper.Map<UserResponse?>(user);
    }

    /// <summary>
    /// Retrieves all tenants associated with a user's email address.
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <returns>A list of tenant responses</returns>
    public async Task<List<TenantResponse>> GetTenantsByUserEmailAsync(string email)
    {
        var users = await _userRepository.GetUsersByEmailAsync(email);
        var tenantIds = users.Select(u => u.TenantId).Distinct().ToList();

        var tenants = new List<Tenant>();
        foreach (var tId in tenantIds)
        {
            var tenant = await _tenantRepository.GetTenantByIdAsync(tId);
            if (tenant != null)
                tenants.Add(tenant);
        }

        return tenants.Select(t => new TenantResponse
        {
            Id = t.TenantId,
            Name = t.TenantName
        }).ToList();
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <returns>The user response if found; otherwise null</returns>
    public async Task<UserResponse?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        return _mapper.Map<UserResponse?>(user);
    }

    /// <summary>
    /// Searches for users based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria including email, first name, tenant ID, and role</param>
    /// <returns>A paged response containing matching users</returns>
    public async Task<PagingResponse<UserResponse>> SearchUsers(Models.User.UserSearchRequest searchRequest)
    {
        var users = await _userRepository.SearchUsers(_mapper.Map<Data.Models.User.UserSearchRequest>(searchRequest));
        return _mapper.Map<PagingResponse<UserResponse>>(users);
    }

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update</param>
    /// <param name="user">The updated user information</param>
    /// <returns>The updated user response</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the user is not found</exception>
    public async Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest user)
    {
        var existing = await _userRepository.GetUserByIdAsync(id)
            ?? throw new KeyNotFoundException($"User not found with id: {id}");

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Email = user.Email;
        existing.TenantId = user.TenantId;

        var updated = await _userRepository.UpdateUserAsync(existing);
        return _mapper.Map<UserResponse>(updated);
    }
}
