using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Services.Caching;

namespace Theatrical.Services.Repositories;

public interface IRoleRepository
{
    Task<List<Role>> GetRoles();
    Task<Role?> GetRole(int id);
    Task CreateRole(Role roles);
    Task DeleteRole(Role roles);
    Task<Role?> GetRoleByName(string searchRole);
    Task UpdateRange(List<Role> roles);
}

public class RoleRepository : IRoleRepository
{
    private readonly TheatricalPlaysDbContext _context;
    private readonly ICaching _caching;

    public RoleRepository(TheatricalPlaysDbContext context, ICaching caching)
    {
        _context = context;
        _caching = caching;
    }

    public async Task<List<Role>> GetRoles()
    {
        var roles = await _caching.GetOrSetAsync("all_roles", async () => await _context.Roles.ToListAsync());
        return roles;
    }

    public async Task<Role?> GetRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);

        return role;
    }

    public async Task<Role?> GetRoleByName(string searchRole)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Role1 == searchRole);
        return role;
    }

    public async Task UpdateRange(List<Role> roles)
    {
        _context.Roles.UpdateRange(roles);
        await _context.SaveChangesAsync();
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