using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IUserRepository
{
    Task<User?> Get(string email);
    Task<User?> Get(int id);
    Task<User?> GetByUsername(string username);
    Task<User> Register(User user, int userRole);
    Task<User?> GetUserIncludingAuthorities(string email);
    Task<decimal> GetUserBalance(int id);
    Task EnableAccount(User user);
    Task<User?> SearchToken(string token);
    Task<User?> SearchOtp(string otp);
    Task Update2Fa(User user, string otp);
    Task Activate2Fa(User user, string userSecret);
    Task Deactivate2Fa(User user);
    Task<User?> GetUserAuthoritiesAndTransactions(string email);
    Task UpdateFacebook(User user, string link);
    Task UpdateInstagram(User user, string link);
    Task UpdateYoutube(User user, string link);
    Task RemoveFacebook(User user);
    Task RemoveYoutube(User user);
    Task RemoveInstagram(User user);
    Task OnRequestApproval(User user, Person person);
    Task UpdateUsername(User user, string username);
}

public class UserRepository : IUserRepository
{
    private readonly TheatricalPlaysDbContext _context;
    private readonly ILogRepository _logRepository;

    public UserRepository(TheatricalPlaysDbContext context, ILogRepository logRepository)
    {
        _context = context;
        _logRepository = logRepository;
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
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
    
    public async Task<User?> GetUserAuthoritiesAndTransactions(string email)
    {
        return await _context.Users
            .Include(u => u.UserAuthorities)
            .Include(u => u.UserTransactions)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task UpdateFacebook(User user, string link)
    {
        user.Facebook = link;
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateInstagram(User user, string link)
    {
        user.Instagram = link;
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateYoutube(User user, string link)
    {
        user.Youtube = link;
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFacebook(User user)
    {
        user.Facebook = null;
        await _context.SaveChangesAsync();
    }

    public async Task RemoveYoutube(User user)
    {
        user.Youtube = null;
        await _context.SaveChangesAsync();
    }

    public async Task RemoveInstagram(User user)
    {
        user.Instagram = null;
        await _context.SaveChangesAsync();
    }

    public async Task OnRequestApproval(User user, Person person)
    {
        user.Username = person.Fullname;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUsername(User user, string username)
    {
        user.Username = username;
        await _context.SaveChangesAsync();
    }

    public async Task<User> Register(User user, int userRole)
    {
        await _context.Users.AddAsync(user);            //adds user
        await _context.SaveChangesAsync();

        await _logRepository.UpdateLogs("insert", "users", new List<(string ColumnName, string Value)>
        {
            ("id", user.Id.ToString()),
            ("email", user.Email)
        });
        
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

        await _logRepository.UpdateLogs("update", "users", new List<(string ColumnName, string Value)>
        {
            ("enabled", $"User {user.Id} has enabled their account through email verification")
        });
    }

    public async Task<User?> SearchToken(string token)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.VerificationCode == token);
    }

    public async Task<User?> SearchOtp(string otp)
    {
        return await _context.Users.Include(u => u.UserAuthorities).FirstOrDefaultAsync(u => u._2FA_code == otp);
    }

    public async Task Update2Fa(User user, string otp)
    {
        user._2FA_code = otp;
        await _context.SaveChangesAsync();
    }

    public async Task Activate2Fa(User user, string userSecret)
    {
        user._2FA_enabled = true;
        user.UserSecret = userSecret;
        await _context.SaveChangesAsync();
    }

    public async Task Deactivate2Fa(User user)
    {
        user._2FA_enabled = false;
        user._2FA_code = null;
        user.UserSecret = null;
        await _context.SaveChangesAsync();
    }
}