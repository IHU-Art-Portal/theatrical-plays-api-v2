using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;
using Theatrical.Services.Repositories;
using BCrypt.Net;

namespace Theatrical.Services;

public interface IUserService
{
    Task<UserDtoRole> Register(UserDto userDto);
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
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
}

