using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Extensions;
using AuthPlus.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthPlus.Identity.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = RoleConstants.AdminRole)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserDto userDto, [FromQuery] string password)
    {
        if (await _userService.UsernameExistsAsync(userDto.UserName))
            return BadRequest("Username already exists");

        await _userService.CreateUserAsync(userDto, password);
        return Ok("User created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto userDto)
    {
        await _userService.UpdateUserAsync(id, userDto);
        return Ok("User updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok("User deleted successfully");
    }

    [HttpPost("{userId}/roles/{roleName}")]
    public async Task<IActionResult> AddRoleToUser(string userId, string roleName)
    {
        await _userService.AddRoleToUserAsync(userId, roleName);
        return Ok("Role added to user successfully");
    }

    [HttpDelete("{userId}/roles/{roleName}")]
    public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
    {
        await _userService.RemoveRoleFromUserAsync(userId, roleName);
        return Ok("Role removed from user successfully");
    }
}
