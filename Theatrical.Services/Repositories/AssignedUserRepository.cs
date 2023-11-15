using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;


public interface IAssignedUserRepository
{
    Task AddAssignedPerson(AssignedUser? assignedUser);
    Task<AssignedUser?> GetByUserId(int userId);
}

public class AssignedUserRepository : IAssignedUserRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public AssignedUserRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task AddAssignedPerson(AssignedUser? assignedUser)
    {
        await _context.AssignedUsers.AddAsync(assignedUser);
        await _context.SaveChangesAsync();
    }

    public async Task<AssignedUser?> GetByUserId(int userId)
    {
        return await _context.AssignedUsers.FirstOrDefaultAsync(au => au.UserId == userId);
    }
}

