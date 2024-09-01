using AuthPlus.Identity.Dtos;
using AuthPlus.Identity.Extensions;
using AuthPlus.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthPlus.Identity.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = RoleConstants.AdminRole)]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    [HttpGet("all")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(string id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        return role == null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto)
    {
        if (await _roleService.RoleExistsAsync(roleDto.Name))
            return BadRequest("Role already exists");

        await _roleService.CreateRoleAsync(roleDto);
        return Ok("Role created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleDto roleDto)
    {
        await _roleService.UpdateRoleAsync(id, roleDto);
        return Ok("Role updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        await _roleService.DeleteRoleAsync(id);
        return Ok("Role deleted successfully");
    }
}
