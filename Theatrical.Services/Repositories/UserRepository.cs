using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IUserRepository
{
    Task<User?> Get(string email);
    Task<User?> Get(int id);
    Task<User> Register(User user, int userRole);
    Task<User?> GetUserIncludingAuthorities(string email);
    Task<decimal> GetUserBalance(int id);
    Task EnableAccount(User user);
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

    public async Task<User?> Get(int id)
    {
        return await _context.Users.FindAsync(id);
    }
    
    public async Task<User?> GetUserIncludingAuthorities(string email)
    {
        return await _context.Users
            .Include(u => u.UserAuthorities)
            .FirstOrDefaultAsync(u => u.Email == email);
    }


    public async Task<User> Register(User user, int userRole)
    {
        await _context.Users.AddAsync(user);            //adds user
        await _context.SaveChangesAsync();

        var userAuthorities = new UserAuthority         //adds user authorities
        {
            UserId = user.Id,
            AuthorityId = userRole                      //1 for admin, 2 for user, 3 for developer
        };

        await _context.UserAuthorities.AddAsync(userAuthorities);
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<decimal> GetUserBalance(int id)
    {
        var credits = await _context.Transactions
            .Where(t => t.UserId == id)
            .SumAsync(t => t.CreditAmount);

        return credits;
    }

    public async Task EnableAccount(User user)
    {
        user.Enabled = true;
        await _context.SaveChangesAsync();
    }
}