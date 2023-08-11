using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;
using Theatrical.Services.Repositories;
using OtpNet;
using Theatrical.Services.Jwt;

namespace Theatrical.Services;

public interface IUserService
{
    Task<UserDtoRole> Register(RegisterUserDto registerUserDto, string verificationToken);
    bool VerifyPassword(string hashedPassword, string providedPassword);
    JwtDto GenerateToken(User user);
    Task EnableAccount(User user);
    string GenerateOTP(User user);
    Task Save2FaCode(User user, string totpCode);
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
        var verificationCodeGuid = Guid.Parse(user.VerificationCode!);

        byte[] secretKeyBytes = verificationCodeGuid.ToByteArray();

        var totp = new Totp(secretKeyBytes);

        var totpCode = totp.ComputeTotp();

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
}

