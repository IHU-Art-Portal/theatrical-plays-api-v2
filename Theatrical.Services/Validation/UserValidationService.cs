using System.Text.RegularExpressions;
using OtpNet;
using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IUserValidationService
{
    Task<ValidationReport> ValidateForRegister(RegisterUserDto userdto);
    Task<(ValidationReport report, User? user)> ValidateForLogin(LoginUserDto loginUserDto);
    Task<(ValidationReport, decimal)> ValidateBalance(int id);
    Task<(ValidationReport, User?)> VerifyEmailToken(string token);
    Task<(ValidationReport, User?)> VerifyOtp(string otpCode);
    Task<(ValidationReport, User?)> ValidateFor2FaDeactivation(LoginUserDto loginUserDto);
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

        if (user._2FA_enabled)
        {
            report.Message = "2FA is already enabled, check your email if you attempted to login.";
            report.Success = true;
            report.ErrorCode = ErrorCode._2FaEnabled;
            return (report, user);
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

    public async Task<(ValidationReport, User?)> VerifyEmailToken(string token)
    {
        var user = await _repository.SearchToken(token);
        var report = new ValidationReport();

        if (user is null)
        {
            report.Success = false;
            report.Message = "Verification Process Failed. Invalid Token";
            report.ErrorCode = ErrorCode.InvalidToken;
            return (report, null);
        }

        if ((bool)user.Enabled!)
        {
            report.Success = true;
            report.Message = "You have already verified your email address";
            report.ErrorCode = ErrorCode.AlreadyVerified;
            return (report, null);
        }

        report.Success = true;
        report.Message = "Verification Completed";

        return (report, user);
    }

    /// <summary>
    /// Verifies the 2fa otp.
    /// </summary>
    /// <param name="otpCode"></param>
    /// <returns>boolean</returns>
    public async Task<(ValidationReport, User?)> VerifyOtp(string otpCode)
    {
        var user = await _repository.SearchOtp(otpCode);
        var report = new ValidationReport();

        if (user is null)
        {
            report.Success = false;
            report.Message = "Authentication failed. User not found";
            report.ErrorCode = ErrorCode._2FaFailure;
            return (report, null);
        }

        byte[] secretKeyBytes = Base32Encoding.ToBytes(user.UserSecret);

        var totp = new Totp(secretKeyBytes);
        
        var window = new VerificationWindow(previous: 1, future: 1);

        bool isCodeValid = totp.VerifyTotp(otpCode, out var timeWindowUsed, VerificationWindow.RfcSpecifiedNetworkDelay);
        
        if (!isCodeValid)
        {
            report.Success = false;
            report.Message = "Authentication failed.";
            report.ErrorCode = ErrorCode._2FaFailure;

            return (report, null);
        }

        report.Success = true;
        report.Message = "Authentication successful!";
        
        return (report, user);
    }

    /// <summary>
    /// Calls login validation.
    /// Completed checks and returns the appropriate ValidationReport model.
    /// </summary>
    /// <param name="loginUserDto"></param>
    /// <returns></returns>
    public async Task<(ValidationReport, User?)> ValidateFor2FaDeactivation(LoginUserDto loginUserDto)
    {
        var (validation, user) = await ValidateForLogin(loginUserDto);
        var validationReport = new ValidationReport();

        if (!validation.Success)
        {
            //If validation in login fails, it returns the validation model and null user.
            return (validation, null);
        }
        
        if (!(user!._2FA_enabled))
        {
            //If user's 2fa is disabled then it returns as error. and null user.
            validationReport.Success = false;
            validationReport.Message = "2FA is not active for your account";
            validationReport.ErrorCode = ErrorCode._2FaDisabled;

            return (validationReport, null);
        }

        //Successful case. 2FA is enabled. thus can be deactivated.
        validationReport.Success = true;
        validationReport.Message = "2FA is active and can be disabled";

        return (validationReport, user);
    }
}