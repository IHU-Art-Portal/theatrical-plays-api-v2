using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IRoleRepository
{
    Task<List<Role>> GetRoles();
    Task<Role> GetRole(int id);
    Task CreateRole(Role role);
    Task DeleteRole(int id);
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

    public async Task<Role> GetRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        return role;
    }

    public async Task CreateRole(Role role)
    {
        await _context.AddAsync(role);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRole(int id)
    {
        var roletodelete = await _context.Roles.FindAsync(id);
        _context.Remove(roletodelete);
        await _context.SaveChangesAsync();
    }
}