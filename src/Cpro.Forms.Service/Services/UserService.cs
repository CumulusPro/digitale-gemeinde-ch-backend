using AutoMapper;
using Cpro.Forms.Data.Models.User;
using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Service.Models.User;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserResponse> AssignUserRoleAsync(Guid id, Models.Enums.Role newRole, string loggedUserEmail)
    {
        var loggerUser = await _userRepository.GetUserByEmailAsync(loggedUserEmail);

        if (loggerUser?.Role != Role.Admin)
            throw new UnauthorizedAccessException("Only Admins can assign roles.");

        var user = await _userRepository.GetUserByIdAsync(id)
            ?? throw new KeyNotFoundException($"User to be updated not found with id: {id}");        

        user.Role = _mapper.Map<Data.Models.User.Role>(newRole);
        var updated = await _userRepository.UpdateUserAsync(user);
        return _mapper.Map<UserResponse>(updated);
    }

    public async Task<UserResponse> CreateUserAsync(UserRequest user)
    {
        var dbUser = _mapper.Map<User>(user);
        dbUser.Id = Guid.NewGuid();

        var saved = await _userRepository.CreateUserAsync(dbUser);
        return _mapper.Map<UserResponse>(saved);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(id)
            ?? throw new KeyNotFoundException($"User not found with id: {id}");

        await _userRepository.DeleteUserAsync(user);
    }

    public async Task<UserResponse?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        return _mapper.Map<UserResponse?>(user);
    }

    public async Task<PagingResponse<UserResponse>> SearchUsers(Models.User.UserSearchRequest searchRequest)
    {
        var users = await _userRepository.SearchUsers(_mapper.Map<Data.Models.User.UserSearchRequest>(searchRequest));
        return _mapper.Map<PagingResponse<UserResponse>>(users);
    }

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
