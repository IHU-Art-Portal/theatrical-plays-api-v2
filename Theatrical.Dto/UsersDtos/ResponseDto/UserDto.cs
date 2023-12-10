using Theatrical.Dto.PersonDtos;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Dto.VenueDtos;

namespace Theatrical.Dto.UsersDtos.ResponseDto;

public class UserDto
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string Email { get; set; } = null!;
    public bool? EmailVerified { get; set; }
    public bool _2FA_enabled { get; set; }
    public string Role { get; set; }
    public List<TransactionDtoFetch>? Transactions { get; set; }
    public string? Facebook { get; set; }
    public string? Youtube { get; set; }
    public string? Instagram { get; set; }
    public decimal? Balance { get; set; }
    public List<string>? PerformerRoles { get; set; }
    public List<UserImageDto>? UserImages { get; set; }
    public UserProfilePictureResponseDto? ProfilePhoto { get; set; }
    public string? BioPdfLocation { get; set; }
    public PersonDto? ClaimedPerson { get; set; }
    public List<VenueDto>? ClaimedVenues { get; set; }
}