using System.Security.Claims;
using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;
using Theatrical.Services.Repositories;
using BCrypt.Net;
using Theatrical.Services.Jwt;

namespace Theatrical.Services;

public interface IUserService
{
    Task<UserDtoRole> Register(RegisterUserDto registerUserDto, string verificationToken);
    bool VerifyPassword(string hashedPassword, string providedPassword);
    JwtDto GenerateToken(User user);
    Task EnableAccount(User user);
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
    /// This function is used to has the password of the user.
    /// Then calls the register of users repository to add the user.
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

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }

    public JwtDto GenerateToken(User user)
    {
        var jwtDto = _tokenService.GenerateToken(user);
        
        return jwtDto;
    }
    
    public async Task EnableAccount(User user)
    {
        await _repository.EnableAccount(user);
    }

}

