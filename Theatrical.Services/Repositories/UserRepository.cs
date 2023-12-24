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
    Task UpdatePassword(User user, string hashedPassword);
    Task UploadPhoto(UserImage userImage);
    Task AddRole(User user, string role);
    Task RemoveRole(User user, string role);
    Task<List<UserImage>?> GetUserImages(int userId);
    Task RemoveUserImage(UserImage userImage);
    Task<UserImage?> GetUserImage(int imageId);
    Task<UserImage?> GetFirstProfileImage(int userId);
    Task UnsetProfilePhoto(UserImage userImage);
    Task SetProfilePhoto(UserImage userImage);
    Task SetBioPdfLocation(User user, string location);
    Task UnsetBio(User user);
    Task UpdateVerifiedPhoneNumber(User user, string number);
    Task RegisterPhoneNumber(User user, string phoneNumber);
    Task RemovePhoneNumber(User user);
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

    public async Task UpdatePassword(User user, string hashedPassword)
    {
        user.Password = hashedPassword;
        await _context.SaveChangesAsync();
    }

    public async Task UploadPhoto(UserImage userImage)
    {
        await _context.UserImages.AddAsync(userImage);
        
        await _context.SaveChangesAsync();
    }

    public async Task AddRole(User user, string role)
    {
        user.PerformerRoles ??= new List<string>();                //same as if (user.Roles == null) user.Roles = new List<string>();
        user.PerformerRoles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRole(User user, string role)
    {
        user.PerformerRoles!.Remove(role);
        
        if (user.PerformerRoles.Count == 0) user.PerformerRoles = null; //Set the artistRoles to null if the list is empty after the removal.
        
        await _context.SaveChangesAsync();
    }

    //Should be used after user confirmation.
    public async Task<List<UserImage>?> GetUserImages(int userId)
    {
        var user = await _context.Users
            .Include(u => u.UserImages)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.UserImages?.ToList();
    }

    public async Task<UserImage?> GetUserImage(int imageId)
    {
        var userImage = await _context.UserImages.FirstOrDefaultAsync(ui => ui.Id == imageId);
        return userImage;
    }

    public async Task<UserImage?> GetFirstProfileImage(int userId)
    {
        var userImage = await _context.UserImages
            .Where(ui => ui.UserId == userId && ui.IsProfile == true)
            .FirstOrDefaultAsync();

        return userImage;
    }

    public async Task UnsetProfilePhoto(UserImage userImage)
    {
        userImage.IsProfile = false;
        await _context.SaveChangesAsync();
    }
    
    public async Task SetProfilePhoto(UserImage userImage)
    {
        userImage.IsProfile = true;
        await _context.SaveChangesAsync();
    }

    //Sets the user's bio_pdf_column to the location.
    //Call this function after obtaining the location of the uploaded file.
    public async Task SetBioPdfLocation(User user, string location)
    {
        user.BioPdfLocation = location;
        await _context.SaveChangesAsync();
    }

    public async Task UnsetBio(User user)
    {
        user.BioPdfLocation = null;
        await _context.SaveChangesAsync();
    }

    public async Task RemoveUserImage(UserImage userImage)
    {
        _context.UserImages.Remove(userImage);
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

    public async Task UpdateVerifiedPhoneNumber(User user, string number)
    {
        user.PhoneNumber = number;
        user.PhoneNumberVerified = true;
        await _context.SaveChangesAsync();
    }

    public async Task RegisterPhoneNumber(User user, string phoneNumber)
    {
        user.PhoneNumber = phoneNumber;
        await _context.SaveChangesAsync();
    }

    public async Task RemovePhoneNumber(User user)
    {
        user.PhoneNumber = null;
        user.PhoneNumberVerified = false;
        await _context.SaveChangesAsync();
    }
}