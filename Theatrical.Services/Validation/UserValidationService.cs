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
    ValidationReport ValidateAuthorizationHeader(string? authorizationHeader);
    Task<(ValidationReport, decimal)> ValidateBalance(int id);
    Task<(ValidationReport, User?)> VerifyEmailToken(string token);
    Task<(ValidationReport, User?)> VerifyOtp(string otpCode);
    Task<(ValidationReport, User?)> ValidateFor2FaDeactivation(string email);
    Task<(ValidationReport, User?)> ValidateFor2FaActivation(string email);
    Task<(ValidationReport, User?)> ValidateUser(string email);
    ValidationReport ValidateSocialMediaLink(string link);
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

    public ValidationReport ValidateAuthorizationHeader(string? authorizationHeader)
    {
        var report = new ValidationReport();
        
        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
        {
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            var principal = _userService.VerifyToken(token);

            if (principal is not null)
            {
                report.Success = false;
                report.Message = "User is already logged in!";
                report.ErrorCode = ErrorCode.AlreadyLoggedIn;

                return report;
            }
        }

        report.Success = true;
        
        return report;
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
            report.Success = false;
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
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<(ValidationReport, User?)> ValidateFor2FaDeactivation(string email)
    {
        var user = await _repository.Get(email);
        var report = new ValidationReport();

        if (user is null)
        {
            report.Success = false;
            report.Message = "Invalid User.";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }
        
        if (!user._2FA_enabled)
        {
            report.Success = false;
            report.Message = "2FA is not enabled for your account.";
            report.ErrorCode = ErrorCode._2FaDisabled;
            return (report, null);
        }
        
        report.Success = true;
        report.Message = "2FA is enabled and can be disabled";

        return (report, user);
    }

    public async Task<(ValidationReport, User?)> ValidateFor2FaActivation(string email)
    {
        var user = await _repository.Get(email);
        var report = new ValidationReport();
        
        if (user is null)
        {
            report.Success = false;
            report.Message = "Invalid User.";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        if (!(bool)user.Enabled!)
        {
            report.Success = false;
            report.Message = "Verify your email in order to activate two factor authentication";
            report.ErrorCode = ErrorCode.InvalidEmail;
            return (report, null);
        }

        if (user._2FA_enabled)
        {
            report.Success = false;
            report.Message = "2FA is already active for your account";
            report.ErrorCode = ErrorCode._2FaEnabled;
            return (report, null);
        }

        report.Success = true;
        report.Message = "2FA is disabled and can be enabled";

        return (report, user);
    }

    public async Task<(ValidationReport, User?)> ValidateUser(string email)
    {
        var user = await _repository.GetUserAuthoritiesAndTransactions(email);
        var report = new ValidationReport();
        
        if (user is null)
        {
            report.Success = false;
            report.Message = "Invalid User.";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Success = true;
        report.Message = "User Found!";
        return (report, user);
    }

    public ValidationReport ValidateSocialMediaLink(string link)
    {
        var report = new ValidationReport();
        
        if ( !(link.Contains("www.youtube.com/channel/") || link.Contains("www.facebook.com/") || link.Contains("www.instagram.com/")) )
        {
            report.Success = false;
            report.Message = "Invalid link";
            report.ErrorCode = ErrorCode.BadRequest;
            return report;
        }

        report.Success = true;
        return report;
    }
}