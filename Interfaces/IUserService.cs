using AuthPlus.Identity.Dtos;

namespace AuthPlus.Identity.Interfaces;
public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(string userId);
    Task<UserDto> GetUserByUsernameAsync(string username);
    Task<UserDto> CreateUserAsync(UserDto userDto, string password);
    Task UpdateUserAsync(string userId, UserDto userDto);
    Task DeleteUserAsync(string userId);
    Task AddRoleToUserAsync(string userId, string roleName);
    Task RemoveRoleFromUserAsync(string userId, string roleName);
    Task<bool> UsernameExistsAsync(string username);
}