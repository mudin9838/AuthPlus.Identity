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
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                ProfileImageUrl = user.ProfileImageUrl,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToArray()
            });
        }

        return userDtos;
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

    public async Task<IdentityResult> UpdateUserAsync(string userId, UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        // Update user details
        user.UserName = userDto.UserName ?? user.UserName;
        user.Email = userDto.Email ?? user.Email;
        user.FullName = userDto.FullName ?? user.FullName;
        user.ProfileImageUrl = userDto.ProfileImageUrl ?? user.ProfileImageUrl;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return result;
        }

        // Handle password change if provided
        if (!string.IsNullOrEmpty(userDto.CurrentPassword) && !string.IsNullOrEmpty(userDto.NewPassword))
        {
            if (userDto.NewPassword != userDto.ConfirmNewPassword)
            {
                return IdentityResult.Failed(new IdentityError { Description = "New passwords do not match." });
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, userDto.CurrentPassword, userDto.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return changePasswordResult;
            }
        }

        return IdentityResult.Success;
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
