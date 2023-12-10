using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;


public interface IAssignedUserRepository
{
    Task AddAssignedPerson(AssignedUser? assignedUser);
    Task<AssignedUser?> GetByUserId(int userId);
    Task<Person?> GetClaimedPersonForUser(int userId);
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

    public async Task<Person?> GetClaimedPersonForUser(int userId)
    {
        var assignedUser = await _context.AssignedUsers.FirstOrDefaultAsync(au => au.UserId == userId);
        
        if (assignedUser is null) return null;
        
        var person = await _context.Persons.FirstOrDefaultAsync(p => p.Id == assignedUser.PersonId);

        return person;
    }
}

