using Cpro.Forms.Service.Models.Enums;
using Cpro.Forms.Service.Models.User;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Peritos.Common.Abstractions.Paging;
using System.Security.Claims;

namespace Cpro.Forms.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Retrieves the current user based on claims and tenant ID.
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>User information if found; otherwise NotFound</returns>
    [HttpGet]
    public async Task<IActionResult> GetUser(int tenantId)
    {
        var claims = _httpContextAccessor.HttpContext?.User?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == "emails")?.Value
                 ?? claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var user = await _userService.GetUserByEmailAndTenantAsync(email, tenantId);
        return user == null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <returns>User information if found; otherwise NotFound</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="userRequest">The user creation request</param>
    /// <returns>The created user information</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserRequest userRequest)
    {
        var result = await _userService.CreateUserAsync(userRequest);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update</param>
    /// <param name="userRequest">The user update request</param>
    /// <returns>The updated user information</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest userRequest)
    {
        var result = await _userService.UpdateUserAsync(id, userRequest);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a user from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete</param>
    /// <returns>NoContent response</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Assigns a new role to a user. Only administrators can perform this operation.
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <param name="newRole">The new role to assign</param>
    /// <returns>The updated user information</returns>
    [HttpPost("{id}/assign-role")]
    public async Task<IActionResult> AssignRole(Guid id, [FromBody] Role newRole)
    {
        var claims = _httpContextAccessor.HttpContext?.User?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == "emails")?.Value
                 ?? claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var updated = await _userService.AssignUserRoleAsync(id, newRole, email);
        return Ok(updated);
    }

    /// <summary>
    /// Searches for users based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria</param>
    /// <returns>Paged response containing matching users</returns>
    [HttpPost("search")]
    public async Task<ActionResult<PagingResponse<UserResponse>>> SearchFormData([FromBody] UserSearchRequest searchRequest)
    {
        var users = await _userService.SearchUsers(searchRequest);
        return Ok(users);
    }

    /// <summary>
    /// Retrieves all tenants associated with the current user's email address.
    /// </summary>
    /// <returns>List of tenant information</returns>
    [HttpGet("tenants")]
    public async Task<IActionResult> GetTenantsByEmail()
    {
        var claims = _httpContextAccessor.HttpContext?.User?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == "emails")?.Value
                 ?? claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var list = await _userService.GetTenantsByUserEmailAsync(email);
        return Ok(list);
    }
}
