using System.Security.Claims;
using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;
using Theatrical.Services.Repositories;
using BCrypt.Net;
using Theatrical.Services.Jwt;

namespace Theatrical.Services;

public interface IUserService
{
    Task<UserDtoRole> Register(UserDto userDto);
    bool VerifyPassword(string hashedPassword, string providedPassword);
    JwtDto GenerateToken(User user);
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
    /// <param name="userDto"></param>
    /// <returns>UserDtoRole</returns>
    public async Task<UserDtoRole> Register(UserDto userDto)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
        
        User user = new User
        {
            Username = userDto.Username,
            Password = hashedPassword,
            Role = "user"
        };
        
        var userCreated = await _repository.Register(user);
        var userDtoRole = new UserDtoRole
        {
            Id = userCreated.Id,
            Username = userCreated.Username,
            Role = userCreated.Role,
            Note = "In order to use POST/DELETE methods your account role must be admin."
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

}

