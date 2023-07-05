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
            Email = userDto.Email,
            Password = hashedPassword,
            Enabled = true
        };

        if (! (userDto.Role.Equals(1) || userDto.Role.Equals(2)) || userDto.Role is null)
        {
            userDto.Role = 2;
        }
        
        var userCreated = await _repository.Register(user, (int)userDto.Role);
        var userDtoRole = new UserDtoRole
        {
            Id = userCreated.Id,
            Email = userCreated.Email,
            Enabled = true
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

