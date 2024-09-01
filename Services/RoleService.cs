using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Entities;
using AuthPlus.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthPlus.Identity.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }
    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = _roleManager.Roles;
        return await roles.Select(role => new RoleDto
        {
            Name = role.Name,
            Description = role.Description
        }).ToListAsync();
    }
    public async Task<RoleDto> GetRoleByIdAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        return role == null ? null : new RoleDto
        {
            Name = role.Name,
            Description = role.Description
        };
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task CreateRoleAsync(RoleDto roleDto)
    {
        var role = new ApplicationRole
        {
            Name = roleDto.Name,
            Description = roleDto.Description
        };

        await _roleManager.CreateAsync(role);
    }

    public async Task UpdateRoleAsync(string id, RoleDto roleDto)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role != null)
        {
            role.Name = roleDto.Name;
            role.Description = roleDto.Description;
            await _roleManager.UpdateAsync(role);
        }
    }

    public async Task DeleteRoleAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role != null)
        {
            await _roleManager.DeleteAsync(role);
        }
    }
}
