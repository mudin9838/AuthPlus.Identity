using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Interfaces;
using AuthPlus.Identity.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthPlus.Identity.Controllers;
[Route("api/[controller]")]
[ApiController]

public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IBaseValidator<UserDto> _userDtoValidator;
    public UserController(IUserService userService, IAuthService authService, IBaseValidator<UserDto> userDtoValidator)
    {
        _userService = userService;
        _authService = authService;
        _userDtoValidator = userDtoValidator;
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token, string userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return BadRequest("Invalid user ID.");
        }

        var result = await _authService.ConfirmEmailAsync(token, userId);
        return result.Succeeded ? Ok("Email confirmed successfully.") : BadRequest("Error confirming email.");
    }
    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
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
        var validationResult = await _userDtoValidator.ValidateAsync(userDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _userService.UpdateUserAsync(id, userDto);
        return result.Succeeded ? Ok("User updated successfully") : BadRequest(result.Errors.Select(e => e.Description));
    }


    [HttpDelete("{id}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok("User deleted successfully");
    }

    [HttpPost("{userId}/roles/{roleName}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> AddRoleToUser(string userId, string roleName)
    {
        await _userService.AddRoleToUserAsync(userId, roleName);
        return Ok("Role added to user successfully");
    }

    [HttpDelete("{userId}/roles/{roleName}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
    {
        await _userService.RemoveRoleFromUserAsync(userId, roleName);
        return Ok("Role removed from user successfully");
    }
}
