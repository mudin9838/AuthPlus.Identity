using AuthPlus.Identity.Dtos;

namespace AuthPlus.Identity.Interfaces;
public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<RoleDto> GetRoleByIdAsync(string id);
    Task<bool> RoleExistsAsync(string roleName);
    Task CreateRoleAsync(RoleDto roleDto);
    Task UpdateRoleAsync(string id, RoleDto roleDto);
    Task DeleteRoleAsync(string id);
}