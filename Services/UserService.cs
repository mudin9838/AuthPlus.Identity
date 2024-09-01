using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Entities;
using AuthPlus.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthPlus.Identity.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user == null ? null : new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FullName = user.FullName,
            ProfileImageUrl = user.ProfileImageUrl
        };
    }

    public async Task<UserDto> GetUserByUsernameAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user == null ? null : new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FullName = user.FullName,
            ProfileImageUrl = user.ProfileImageUrl
        };
    }

    public async Task<UserDto> CreateUserAsync(UserDto userDto, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userDto.UserName,
            Email = userDto.Email,
            FullName = userDto.FullName,
            ProfileImageUrl = userDto.ProfileImageUrl
        };

        await _userManager.CreateAsync(user, password);
        return userDto;
    }

    public async Task UpdateUserAsync(string userId, UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.FullName = userDto.FullName;
            user.ProfileImageUrl = userDto.ProfileImageUrl;
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
    }

    public async Task AddRoleToUserAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }
    }

    public async Task RemoveRoleFromUserAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.RemoveFromRoleAsync(user, roleName);
        }
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _userManager.FindByNameAsync(username) != null;
    }
}
