using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IRoleRepository
{
    Task<List<Roles>> GetRoles();
    Task<Roles?> GetRole(int id);
    Task CreateRole(Roles roles);
    Task DeleteRole(Roles roles);
    Task<Roles?> GetRoleByName(string searchRole);
}

public class RoleRepository : IRoleRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public RoleRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task<List<Roles>> GetRoles()
    {
        var roles = await _context.Roles.ToListAsync();
        return roles;
    }

    public async Task<Roles?> GetRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);

        return role;
    }

    public async Task<Roles?> GetRoleByName(string searchRole)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Role == searchRole);
        return role;
    }

    public async Task CreateRole(Roles roles)
    {
        await _context.AddAsync(roles);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRole(Roles roles)
    {
        _context.Remove(roles);
        await _context.SaveChangesAsync();
    }
}