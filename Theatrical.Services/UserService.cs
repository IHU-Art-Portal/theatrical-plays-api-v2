using System.Security.Claims;
using Theatrical.Data.Models;
using Theatrical.Services.Repositories;
using OtpNet;
using Theatrical.Data.enums;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Dto.UsersDtos;
using Theatrical.Dto.UsersDtos.ResponseDto;
using Theatrical.Services.Security.Jwt;

namespace Theatrical.Services;

public interface IUserService
{
    Task<UserDtoRole> Register(RegisterUserDto registerUserDto, string verificationToken);
    bool VerifyPassword(string hashedPassword, string providedPassword);
    JwtDto GenerateToken(User user);
    Task EnableAccount(User user);
    string GenerateOTP(User user);
    Task Save2FaCode(User user, string totpCode);
    Task ActivateTwoFactorAuthentication(User user);
    Task DeactivateTwoFactorAuthentication(User user);
    ClaimsPrincipal? VerifyToken(string token);
    Task<UserDto> ToDto(User user);
    Task UpdateFacebook(User user, string link);
    Task UpdateInstagram(User user, string link);
    Task UpdateYoutube(User user, string link);
    Task RemoveSocialMedia(User user, SocialMedia socialMedia);
    Task UpdateUsername(UpdateUsernameDto updateUsernameDto);
    Task UpdatePassword(UpdatePasswordDto updatePasswordDto, User user);
    Task<string> SetTemporaryPassword(User user);
    Task UploadPhoto(User user, UpdateUserPhotoDto updateUserPhotoDto);
    Task AddRole(User user, string role);
    Task RemoveRole(User user, string role);
    Task SetProfilePhoto(User user, UserImage userImage, SetProfilePhotoDto setProfilePhotoDto);
    Task RemoveUserImage(UserImage userImage);
    Task UnsetProfilePhoto(UserImage userImage);
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
    /// Creates a hashed password based on the user's actual password
    /// Some logic...
    /// Calls the repository method to register the user.
    /// Returns a Dto Reply.
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

    /// <summary>
    /// Verifies the hashed passwords.
    /// </summary>
    /// <param name="hashedPassword">The correct password.</param>
    /// <param name="providedPassword">The provided password by the user who attempted to log in.</param>
    /// <returns></returns>
    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }

    /// <summary>
    /// Generates a jwt token.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public JwtDto GenerateToken(User user)
    {
        var jwtDto = _tokenService.GenerateToken(user);
        
        return jwtDto;
    }
    
    /// <summary>
    /// Enabled a account.
    /// Triggered after clicking the email link.
    /// </summary>
    /// <param name="user"></param>
    public async Task EnableAccount(User user)
    {
        await _repository.EnableAccount(user);
    }

    /// <summary>
    /// Generates a new 2fa code.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public string GenerateOTP(User user)
    {
        byte[] secretKeyBytes = Base32Encoding.ToBytes(user.UserSecret);

        var totp = new Totp(secretKeyBytes);

        var totpCode = totp.ComputeTotp(DateTime.UtcNow);

        return totpCode;
    }

    /// <summary>
    /// Saves the temporary 2fa code to the db.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="totpCode"></param>
    public async Task Save2FaCode(User user, string totpCode)
    {
        await _repository.Update2Fa(user, totpCode);
    }

    /// <summary>
    /// Activates 2fa.
    /// </summary>
    /// <param name="user"></param>
    public async Task ActivateTwoFactorAuthentication(User user)
    {
        var secretKeyBytes = KeyGeneration.GenerateRandomKey(20);

        var hexString = Base32Encoding.ToString(secretKeyBytes);
        
        await _repository.Activate2Fa(user, hexString);
    }

    /// <summary>
    /// Deactivates 2fa.
    /// </summary>
    /// <param name="user"></param>
    public async Task DeactivateTwoFactorAuthentication(User user)
    {
        await _repository.Deactivate2Fa(user);
    }

    public ClaimsPrincipal? VerifyToken(string token)
    {
        return _tokenService.VerifyToken(token);
    }

    public async Task<UserDto> ToDto(User user)
    {

        var userIntRole = user.UserAuthorities.FirstOrDefault()?.AuthorityId;
        var userImages = await _repository.GetUserImages(user.Id);
        var userImagesDto = new List<UserImagesDto>();
        var userProfilePhoto = await _repository.GetFirstProfileImage(user.Id);
        var userProfilePictureResponseDto = new UserProfilePictureResponseDto();

        if (userProfilePhoto is not null)
        {
            userProfilePictureResponseDto.Id = userProfilePhoto.Id;
            userProfilePictureResponseDto.ImageLocation = userProfilePhoto.ImageLocation;
            userProfilePictureResponseDto.Label = userProfilePhoto.Label;
        }
        
        if (userImages is not null)
        {
            foreach (var userImage in userImages)
            {
                var userImageDto = new UserImagesDto
                {
                    Id = userImage.Id,
                    ImageLocation = userImage.ImageLocation,
                    Label = userImage.Label
                };
                userImagesDto.Add(userImageDto);
            }
        }

        string userRole = userIntRole switch
        {
            1 => "admin",
            2 => "user",
            3 => "developer",
            4 => "claims manager",
            _ => "Invalid role"
        };

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            EmailVerified = user.Enabled,
            _2FA_enabled = user._2FA_enabled,
            Role = userRole,
            Transactions = user.UserTransactions.Select(t => new TransactionDtoFetch
            {
                UserId = t.UserId,
                CreditAmount = t.CreditAmount,
                Reason = t.Reason,
                DateCreated = t.DateCreated,
                TransactionId = t.TransactionId.ToString(),
                AuthCode = t.AuthCode,
                AccountNumber = t.AccountNumber,
                AccountType = t.AccountType
            }).ToList(),
            Facebook = user.Facebook,
            Youtube = user.Youtube,
            Instagram = user.Instagram,
            Balance = user.UserTransactions.Sum(t => t.CreditAmount),
            UserImages = userImagesDto,
            ProfilePhoto = userProfilePictureResponseDto,
            PerformerRoles = user.PerformerRoles
        };

        return userDto;
    }

    public async Task UpdateFacebook(User user, string link)
    {
        await _repository.UpdateFacebook(user, link);
    }
    
    public async Task UpdateYoutube(User user, string link)
    {
        await _repository.UpdateYoutube(user, link);
    }
    
    public async Task UpdateInstagram(User user, string link)
    {
        await _repository.UpdateInstagram(user, link);
    }
    
    public async Task RemoveSocialMedia(User user, SocialMedia socialMedia)
    {
        if (socialMedia == SocialMedia.Facebook)
        {
            await _repository.RemoveFacebook(user);
            return;
        }

        if (socialMedia == SocialMedia.Youtube)
        {
            await _repository.RemoveYoutube(user);
            return;
        }

        await _repository.RemoveInstagram(user);
    }

    public async Task UpdateUsername(UpdateUsernameDto updateUsernameDto)
    {
        await _repository.UpdateUsername(updateUsernameDto.User, updateUsernameDto.Username);
    }
    
    public async Task UpdatePassword(UpdatePasswordDto updatePasswordDto, User user)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(updatePasswordDto.Password);
        await _repository.UpdatePassword(user, hashedPassword);
    }

    public async Task<string> SetTemporaryPassword(User user)
    {
        var tempPassword = Guid.NewGuid().ToString();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(tempPassword); 
        await _repository.UpdatePassword(user, hashedPassword);
        return tempPassword;
    }

    public async Task UploadPhoto(User user, UpdateUserPhotoDto updateUserPhotoDto)
    {
        if (updateUserPhotoDto.IsProfile)
        {
            var userImageProfile = await _repository.GetFirstProfileImage(user.Id);
            if (userImageProfile is not null)
            {
                await _repository.UnsetProfilePhoto(userImageProfile);
            }
        }
        
        var userImage = new UserImage
        {
            ImageLocation = updateUserPhotoDto.Photo,
            Label = updateUserPhotoDto.Label,
            UserId = user.Id,
            IsProfile = updateUserPhotoDto.IsProfile
        };
        
        await _repository.UploadPhoto(userImage);
    }

    public async Task AddRole(User user, string role)
    {
        await _repository.AddRole(user, role);
    }

    public async Task RemoveRole(User user, string role)
    {
        await _repository.RemoveRole(user, role);
    }

    public async Task SetProfilePhoto(User user, UserImage userImage, SetProfilePhotoDto setProfilePhotoDto)
    {
        var userImageProfile = await _repository.GetFirstProfileImage(user.Id);
        
        if (userImageProfile is not null)
        {
            //unsets the current profile picture if found.
            await _repository.UnsetProfilePhoto(userImageProfile);
        }

        userImage.Label = setProfilePhotoDto.Label;

        await _repository.SetProfilePhoto(userImage);
    }

    public async Task RemoveUserImage(UserImage userImage)
    {
        await _repository.RemoveUserImage(userImage);
    }

    public async Task UnsetProfilePhoto(UserImage userImage)
    {
        await _repository.UnsetProfilePhoto(userImage);
    }
}

