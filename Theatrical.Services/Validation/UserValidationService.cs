using System.Security.Claims;
using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;
using Theatrical.Services.Jwt;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IUserValidationService
{
    Task<ValidationReport> ValidateForRegister(UserDto userdto);
    Task<(ValidationReport report, User user)> ValidateForLogin(UserDto userDto);
    ValidationReport ValidateUser(string? jwtToken);
}

public class UserValidationService : IUserValidationService
{
    private readonly IUserRepository _repository;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public UserValidationService(IUserRepository repository, IUserService userService, ITokenService tokenService)
    {
        _repository = repository;
        _userService = userService;
        _tokenService = tokenService;
    }

    public async Task<ValidationReport> ValidateForRegister(UserDto userdto)
    {
        var report = new ValidationReport();
        var user = await _repository.Get(userdto.Email);

        if (user is not null)
        {
            report.Message = "Username taken!";
            report.Success = false;
            return report;
        }

        report.Message = "User with this username can be created";
        report.Success = true;
        return report;
    }

    public async Task<(ValidationReport report, User user)> ValidateForLogin(UserDto userDto)
    {
        var report = new ValidationReport();
        var user = await _repository.Get(userDto.Email);

        if (user is null)
        {
            report.Message = "User not found";
            report.Success = false;
            return (report, null);
        }

        if (!_userService.VerifyPassword(user.Password, userDto.Password))
        {
            report.Message = "User with this combination not found";
            report.Success = false;
            return (report, null);
        }
        else
        {
            report.Message = "User Verified";
            report.Success = true;
            return (report, user);
        }

    }

    public ValidationReport ValidateUser(string? jwtToken)
    {
        var report = new ValidationReport();

        if (jwtToken is null)
        {
            report.Success = false;
            report.Message = "You did not provide a JWT token";
            return report;
        }

        var principal = _tokenService.VerifyToken(jwtToken);

        if (principal is null)
        {
            report.Success = false;
            report.Message = "Invalid token";
            return report;
        }

        if (!principal.IsInRole("admin"))
        {
            report.Success = false;
            report.Message = "User is forbidden for changes";
            return report;
        }
        
        report.Success = true;
        report.Message = "User is authorized for changes";
        return report;
    }
}