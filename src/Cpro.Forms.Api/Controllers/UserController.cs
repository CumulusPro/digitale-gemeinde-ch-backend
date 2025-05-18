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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserRequest userRequest)
    {
        var result = await _userService.CreateUserAsync(userRequest);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest userRequest)
    {
        var result = await _userService.UpdateUserAsync(id, userRequest);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/assign-role")]
    public async Task<IActionResult> AssignRole(Guid id, [FromBody] Role newRole)
    {
        var claims = _httpContextAccessor.HttpContext?.User?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == "emails")?.Value
                 ?? claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var updated = await _userService.AssignUserRoleAsync(id, newRole, email);
        return Ok(updated);
    }

    [HttpPost("search")]
    public async Task<ActionResult<PagingResponse<UserResponse>>> SearchFormData([FromBody] UserSearchRequest searchRequest)
    {
        var users = await _userService.SearchUsers(searchRequest);
        return Ok(users);
    }
}
