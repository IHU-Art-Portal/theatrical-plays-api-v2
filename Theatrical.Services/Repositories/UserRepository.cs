using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IUserRepository
{
    Task<User?> Get(string email);
    Task<User> Register(User user);
}

public class UserRepository : IUserRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public UserRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task<User?> Get(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> Register(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        var userCreated = await Get(user.Email);
        return userCreated;
    }
}