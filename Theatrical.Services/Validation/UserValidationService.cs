using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IUserValidationService
{
    Task<ValidationReport> ValidateForRegister(UserDto userdto);
    Task<(ValidationReport report, User user)> ValidateForLogin(UserDto userDto);
}

public class UserValidationService : IUserValidationService
{
    private readonly IUserRepository _repository;
    private readonly IUserService _userService;

    public UserValidationService(IUserRepository repository, IUserService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    public async Task<ValidationReport> ValidateForRegister(UserDto userdto)
    {
        var report = new ValidationReport();
        var user = await _repository.Get(userdto.Username);

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
        var user = await _repository.Get(userDto.Username);

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
}