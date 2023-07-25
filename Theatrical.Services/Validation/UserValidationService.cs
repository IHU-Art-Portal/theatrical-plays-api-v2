using System.Text.RegularExpressions;
using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Jwt;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IUserValidationService
{
    Task<ValidationReport> ValidateForRegister(RegisterUserDto userdto);
    Task<(ValidationReport report, User? user)> ValidateForLogin(LoginUserDto loginUserDto);
    Task<(ValidationReport, decimal)> ValidateBalance(int id);
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

    public async Task<ValidationReport> ValidateForRegister(RegisterUserDto userdto)
    {
        var report = new ValidationReport();
        
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if (!Regex.IsMatch(userdto.Email, emailPattern))
        {
            report.Success = false;
            report.Message = "Please provide a valid email!";
            report.ErrorCode = ErrorCode.InvalidEmail;
            return report;
        }
        
        var user = await _repository.Get(userdto.Email);

        if (user is not null)
        {
            report.Message = "email field already exists!";
            report.Success = false;
            report.ErrorCode = ErrorCode.AlreadyExists;
            return report;
        }

        report.Message = "User with this email can be created";
        report.Success = true;
        return report;
    }

    public async Task<(ValidationReport report, User? user)> ValidateForLogin(LoginUserDto loginUserDto)
    {
        var report = new ValidationReport();
        var user = await _repository.GetUserIncludingAuthorities(loginUserDto.Email);
        
        if (user is null)
        {
            report.Message = "User not found";
            report.Success = false;
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        if (!_userService.VerifyPassword(user.Password!, loginUserDto.Password))
        {
            report.Message = "User with this combination not found";
            report.Success = false;
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }
        
        report.Message = "User Verified";
        report.Success = true;
        return (report, user);

    }

    public async Task<(ValidationReport, decimal)> ValidateBalance(int id)
    {
        var report = new ValidationReport();
        var user = await _repository.Get(id);

        if (user is null)
        {
            report.Message = "User not found";
            report.Success = false;
            report.ErrorCode = ErrorCode.NotFound;
            return (report, 0m);
        }

        var credits = await _repository.GetUserBalance(id);
        
        report.Success = true;
        report.Message = "Success";
        
        return (report, credits);
    }
}