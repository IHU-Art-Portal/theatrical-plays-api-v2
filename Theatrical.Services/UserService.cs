using System.Security.Claims;
using Theatrical.Data.Models;
using Theatrical.Services.Repositories;
using OtpNet;
using Theatrical.Data.enums;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Dto.UsersDtos;
using Theatrical.Dto.UsersDtos.ResponseDto;
using Theatrical.Services.Security.Jwt;

namespace Theatrical.Services;

public interface IUserService
{
    Task<UserDtoRole> Register(RegisterUserDto registerUserDto, string verificationToken);
    bool VerifyPassword(string hashedPassword, string providedPassword);
    JwtDto GenerateToken(User user);
    Task EnableAccount(User user);
    string GenerateOTP(User user);
    Task Save2FaCode(User user, string totpCode);
    Task ActivateTwoFactorAuthentication(User user);
    Task DeactivateTwoFactorAuthentication(User user);
    ClaimsPrincipal? VerifyToken(string token);
    UserDto ToDto(User user);
    Task UpdateFacebook(User user, string link);
    Task UpdateInstagram(User user, string link);
    Task UpdateYoutube(User user, string link);
    Task RemoveSocialMedia(User user, SocialMedia socialMedia);
    Task UpdateUsername(UpdateUsernameDto updateUsernameDto);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository repository, ITokenService tokenService)
    {
        _repository = repository;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Creates a hashed password based on the user's actual password
    /// Some logic...
    /// Calls the repository method to register the user.
    /// Returns a Dto Reply.
    /// </summary>
    /// <param name="registerUserDto"></param>
    /// <param name="verificationToken"></param>
    /// <returns>UserDtoRole</returns>
    public async Task<UserDtoRole> Register(RegisterUserDto registerUserDto, string verificationToken)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);

        User user = new User
        {
            Email = registerUserDto.Email,
            Password = hashedPassword,
            Enabled = false,
            VerificationCode = verificationToken,
            _2FA_enabled = false
        };

        if (! (registerUserDto.Role.Equals(1) || registerUserDto.Role.Equals(2) || registerUserDto.Role.Equals(3)) || registerUserDto.Role is null)
        {
            registerUserDto.Role = 2;
        }
        
        var userCreated = await _repository.Register(user, (int)registerUserDto.Role);
        var userDtoRole = new UserDtoRole
        {
            Id = userCreated.Id,
            Email = userCreated.Email,
            Enabled = (bool)userCreated.Enabled!,
            Note = "Check your email for the verification link to enable your account!"
        };

        return userDtoRole;
    }

    /// <summary>
    /// Verifies the hashed passwords.
    /// </summary>
    /// <param name="hashedPassword">The correct password.</param>
    /// <param name="providedPassword">The provided password by the user who attempted to log in.</param>
    /// <returns></returns>
    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }

    /// <summary>
    /// Generates a jwt token.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public JwtDto GenerateToken(User user)
    {
        var jwtDto = _tokenService.GenerateToken(user);
        
        return jwtDto;
    }
    
    /// <summary>
    /// Enabled a account.
    /// Triggered after clicking the email link.
    /// </summary>
    /// <param name="user"></param>
    public async Task EnableAccount(User user)
    {
        await _repository.EnableAccount(user);
    }

    /// <summary>
    /// Generates a new 2fa code.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public string GenerateOTP(User user)
    {
        byte[] secretKeyBytes = Base32Encoding.ToBytes(user.UserSecret);

        var totp = new Totp(secretKeyBytes);

        var totpCode = totp.ComputeTotp(DateTime.UtcNow);

        return totpCode;
    }

    /// <summary>
    /// Saves the temporary 2fa code to the db.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="totpCode"></param>
    public async Task Save2FaCode(User user, string totpCode)
    {
        await _repository.Update2Fa(user, totpCode);
    }

    /// <summary>
    /// Activates 2fa.
    /// </summary>
    /// <param name="user"></param>
    public async Task ActivateTwoFactorAuthentication(User user)
    {
        var secretKeyBytes = KeyGeneration.GenerateRandomKey(20);

        var hexString = Base32Encoding.ToString(secretKeyBytes);
        
        await _repository.Activate2Fa(user, hexString);
    }

    /// <summary>
    /// Deactivates 2fa.
    /// </summary>
    /// <param name="user"></param>
    public async Task DeactivateTwoFactorAuthentication(User user)
    {
        await _repository.Deactivate2Fa(user);
    }

    public ClaimsPrincipal? VerifyToken(string token)
    {
        return _tokenService.VerifyToken(token);
    }

    public UserDto ToDto(User user)
    {

        var userIntRole = user.UserAuthorities.FirstOrDefault()?.AuthorityId;

        string userRole = userIntRole switch
        {
            1 => "admin",
            2 => "user",
            3 => "developer",
            4 => "claims manager",
            _ => "Invalid role"
        };

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            EmailVerified = user.Enabled,
            _2FA_enabled = user._2FA_enabled,
            Role = userRole,
            Transactions = user.UserTransactions.Select(t => new TransactionDtoFetch
            {
                UserId = t.UserId,
                CreditAmount = t.CreditAmount,
                Reason = t.Reason,
                DateCreated = t.DateCreated,
                TransactionId = t.TransactionId.ToString(),
                AuthCode = t.AuthCode,
                AccountNumber = t.AccountNumber,
                AccountType = t.AccountType
            }).ToList(),
            Facebook = user.Facebook,
            Youtube = user.Youtube,
            Instagram = user.Instagram,
            Balance = user.UserTransactions.Sum(t => t.CreditAmount)
        };

        return userDto;
    }

    public async Task UpdateFacebook(User user, string link)
    {
        await _repository.UpdateFacebook(user, link);
    }
    
    public async Task UpdateYoutube(User user, string link)
    {
        await _repository.UpdateYoutube(user, link);
    }
    
    public async Task UpdateInstagram(User user, string link)
    {
        await _repository.UpdateInstagram(user, link);
    }
    
    public async Task RemoveSocialMedia(User user, SocialMedia socialMedia)
    {
        if (socialMedia == SocialMedia.Facebook)
        {
            await _repository.RemoveFacebook(user);
            return;
        }

        if (socialMedia == SocialMedia.Youtube)
        {
            await _repository.RemoveYoutube(user);
            return;
        }

        await _repository.RemoveInstagram(user);
    }

    public async Task UpdateUsername(UpdateUsernameDto updateUsernameDto)
    {
        await _repository.UpdateUsername(updateUsernameDto.User, updateUsernameDto.Username);
    }
}

