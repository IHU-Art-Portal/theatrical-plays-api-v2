using System.Text.RegularExpressions;
using OtpNet;
using Theatrical.Data.enums;
using Theatrical.Data.Models;
using Theatrical.Dto.UsersDtos;
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
    Task<(ValidationReport, User?)> ValidateUserById(int userId);
    ValidationReport ValidateYoutubeLink(string link);
    ValidationReport ValidateFacebookLink(string link);
    ValidationReport ValidateInstagramLink(string link);
    ValidationReport ValidateSocialMediaForDelete(User user, SocialMedia socialMedia);
    Task<ValidationReport> ValidateUniqueUsername(string username);
    Task<ValidationReport> ValidateUserRole(User user, string role);
    Task<ValidationReport> ValidateForRemoveRole(User user, string role);
    Task<User?> ValidateWithEmail(string email);
    Task<ValidationReport> ValidateUserPersonUniqueness(int userId);            //Validates User-Person relation uniqueness.
    Task<(ValidationReport, UserImage?)> ValidateUserImageExistence(int imageId);
    ValidationReport ValidatePhotoOwnership(User user, UserImage userImage);
    ValidationReport ValidateBioExistence(User user);
    ValidationReport ValidateNumber(User user);
}

public class UserValidationService : IUserValidationService
{
    private readonly IUserRepository _repository;
    private readonly IUserService _userService;
    private readonly IAssignedUserRepository _assignedUsersRepository;

    public UserValidationService(IUserRepository repository, IUserService userService, IAssignedUserRepository assignedUserRepository)
    {
        _repository = repository;
        _userService = userService;
        _assignedUsersRepository = assignedUserRepository;
    }
    
    public async Task<User?> ValidateWithEmail(string email)
    {
        var user = await _repository.Get(email);
        return user;
    }

    public async Task<ValidationReport> ValidateUserPersonUniqueness(int userId)
    {
        var assignedUser = await _assignedUsersRepository.GetByUserId(userId);
        var report = new ValidationReport();

        if (assignedUser is not null)
        {
            report.Message = "You already have an approved request. You can't make more person account requests.";
            report.Success = false;
            report.ErrorCode = ErrorCode.AlreadyExists;
            return report;
        }

        report.Success = true;
        report.Message = "User does not have an approved request and may create additional requests.";
        return report;
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

        //The provided password by the user who attempted to log in, and, the correct hashed password.
        if (!BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.Password!)) 
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

    public async Task<ValidationReport> ValidateUniqueUsername(string username)
    {
        var user = await _repository.GetByUsername(username);
        var report = new ValidationReport();
        
        if (user is not null && !(bool)user.Enabled!)
        {
            report.Success = false;
            report.Message = "Verify your email in order to set your username.";
            report.ErrorCode = ErrorCode.InvalidEmail;
            return report;
        }
        
        if (user is null)
        {
            report.Success = true;
            report.Message = "No user found with this username. You may use this username.";
            return report;
        }
        
        

        report.Success = false;
        report.Message = "Username must be unique";
        report.ErrorCode = ErrorCode.AlreadyExists;
        return report;
    }

    public async Task<ValidationReport> ValidateUserRole(User user, string role)
    {
        var report = new ValidationReport();
        
        //Check for special characters. Roles can only include letters english/greek and spaces.
        if (!Regex.Match(role, @"^[a-zA-ZΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff\s]+$").Success)
        {
            report.Message = "Role cannot contain special characters or numbers.";
            report.Success = false;
            report.ErrorCode = ErrorCode.InvalidCharacters;
            return report;
        }
        
        if (user.PerformerRoles == null)
        {
            report.Message = "No user roles yet. This role can be added.";
            report.Success = true;
            return report;
        }

        if (user.PerformerRoles.Contains(role))
        {
            report.Message = "This role already exists.";
            report.Success = false;
            report.ErrorCode = ErrorCode.AlreadyExists;
            return report;
        }

        report.Success = true;
        report.Message = "Role can be added.";
        return report;
    }

    public async Task<ValidationReport> ValidateForRemoveRole(User user, string role)
    {
        var report = new ValidationReport();

        if (user.PerformerRoles is null)
        {
            report.Success = false;
            report.Message = "You don't have any roles.";
            report.ErrorCode = ErrorCode.BadRequest;
            return report;
        }
        
        if (!user.PerformerRoles.Contains(role))
        {
            report.Success = false;
            report.Message = $"You don't have a role with the name: {role}";
            report.ErrorCode = ErrorCode.NotFound;
            return report;
        }

        report.Success = true;
        report.Message = "Role can be removed.";
        return report;
    }

    public async Task<(ValidationReport, User?)> ValidateUserById(int userId)
    {
        var user = await _repository.Get(userId);
        var report = new ValidationReport();
        
        if (user is null)
        {
            report.Success = false;
            report.Message = "This user has deleted their account.";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Success = true;
        report.Message = "User Found!";

        return (report, user);
    }

    public ValidationReport ValidateYoutubeLink(string link)
    {
        var report = new ValidationReport();

        if (!Uri.IsWellFormedUriString(link, UriKind.Absolute))
        {
            //the Uri.IsWellFormedUriString() method is used to validate the link. If the link is not well-formed
            //(i.e., it doesn't have a valid URI format), it will be considered invalid.
            report.Success = false;
            report.Message = "Invalid link";
            report.ErrorCode = ErrorCode.BadRequest;
            return report;
        }
        
        if ( !link.Contains("www.youtube.com/channel/") )
        {
            //Additionally, the code still
            //checks if the link contains the required substrings for YouTube, Facebook, or Instagram profiles.
            report.Success = false;
            report.Message = "Invalid link";
            report.ErrorCode = ErrorCode.BadRequest;
            return report;
        }

        report.Success = true;
        return report;
    }
    
    public ValidationReport ValidateFacebookLink(string link)
    {
        var report = new ValidationReport();

        if (!Uri.IsWellFormedUriString(link, UriKind.Absolute))
        {
            //the Uri.IsWellFormedUriString() method is used to validate the link. If the link is not well-formed
            //(i.e., it doesn't have a valid URI format), it will be considered invalid.
            report.Success = false;
            report.Message = "Invalid link";
            report.ErrorCode = ErrorCode.BadRequest;
            return report;
        }
        
        if (!link.Contains("www.facebook.com/"))
        {
            //Additionally, the code still
            //checks if the link contains the required substrings for YouTube, Facebook, or Instagram profiles.
            report.Success = false;
            report.Message = "Invalid link";
            report.ErrorCode = ErrorCode.BadRequest;
            return report;
        }

        report.Success = true;
        return report;
    }
    
    public ValidationReport ValidateInstagramLink(string link)
    {
        var report = new ValidationReport();

        if (!Uri.IsWellFormedUriString(link, UriKind.Absolute))
        {
            //the Uri.IsWellFormedUriString() method is used to validate the link. If the link is not well-formed
            //(i.e., it doesn't have a valid URI format), it will be considered invalid.
            report.Success = false;
            report.Message = "Invalid link";
            report.ErrorCode = ErrorCode.BadRequest;
            return report;
        }
        
        if (!link.Contains("www.instagram.com/"))
        {
            //Additionally, the code still
            //checks if the link contains the required substrings for YouTube, Facebook, or Instagram profiles.
            report.Success = false;
            report.Message = "Invalid link";
            report.ErrorCode = ErrorCode.BadRequest;
            return report;
        }

        report.Success = true;
        return report;
    }

    public ValidationReport ValidateSocialMediaForDelete(User user, SocialMedia socialMedia)
    {
        if (socialMedia == SocialMedia.Facebook)
        {
            if (string.IsNullOrEmpty(user.Facebook))
            {
                return new ValidationReport
                {
                    Success = false,
                    ErrorCode = ErrorCode.NotFound,
                    Message = "You don't have a facebook account registered."
                };
            }

            return new ValidationReport
            {
                Success = true,
                Message = "Facebook account found and can be deleted."
            };
        }

        if (socialMedia == SocialMedia.Youtube)
        {
            if (string.IsNullOrEmpty(user.Youtube))
            {
                return new ValidationReport
                {
                    Success = false,
                    ErrorCode = ErrorCode.NotFound,
                    Message = "You don't have a youtube account registered."
                };
            }

            return new ValidationReport
            {
                Success = true,
                Message = "Youtube account found and can be deleted."
            };
        }

        if (string.IsNullOrEmpty(user.Instagram))
        {
            return new ValidationReport
            {
                Success = false,
                ErrorCode = ErrorCode.NotFound,
                Message = "You don't have a instagram account registered."
            };
        }

        return new ValidationReport
        {
            Success = true,
            Message = "Instagram account found and can be deleted."
        };
    }

    public async Task<(ValidationReport, UserImage?)> ValidateUserImageExistence(int imageId)
    {
        var userImage = await _repository.GetUserImage(imageId);
        var report = new ValidationReport();

        if (userImage is null)
        {
            report.Success = false;
            report.Message = "Image not found.";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Success = true;
        return (report, userImage);
    }

    public ValidationReport ValidatePhotoOwnership(User user, UserImage userImage)
    {
        var report = new ValidationReport();
        
        if (userImage.UserId != user.Id)
        {
            report.Success = false;
            report.Message = "You can't review other people's images";
            report.ErrorCode = ErrorCode.Forbidden;
            return report;
        }

        report.Success = true;
        report.Message = "Validation passed";
        return report;
    }

    public ValidationReport ValidateBioExistence(User user)
    {
        var report = new ValidationReport();
        
        if (user.BioPdfLocation is null)
        {
            report.Success = false;
            report.Message = "Bio does not exist";
            report.ErrorCode = ErrorCode.NotFound;
            return report;
        }

        report.Success = true;
        report.Message = "Bio exists!";
        return report;
    }

    public ValidationReport ValidateNumber(User user)
    {
        var report = new ValidationReport();
        
        if (user.PhoneNumber is not null)
        {
            report.Message = "User already has a registered number";
            report.Success = false;
            report.ErrorCode = ErrorCode.BadRequest;
            return report;
        }

        report.Success = true;
        return report;
    }
}