using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IRoleRepository
{
    Task<List<Role>> GetRoles();
    Task<Role?> GetRole(int id);
    Task CreateRole(Role role);
    Task DeleteRole(Role role);
    Task<Role?> GetRoleByName(string searchRole);
}

public class RoleRepository : IRoleRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public RoleRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetRoles()
    {
        var roles = await _context.Roles.ToListAsync();
        return roles;
    }

    public async Task<Role?> GetRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);

        return role;
    }

    public async Task<Role?> GetRoleByName(string searchRole)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Value == searchRole);
        return role;
    }

    public async Task CreateRole(Role role)
    {
        await _context.AddAsync(role);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRole(Role role)
    {
        _context.Remove(role);
        await _context.SaveChangesAsync();
    }
}