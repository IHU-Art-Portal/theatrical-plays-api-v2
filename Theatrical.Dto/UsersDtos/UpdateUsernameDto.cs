using Theatrical.Data.Models;

namespace Theatrical.Dto.UsersDtos;

public class UpdateUsernameDto
{
    public User User { get; set; }
    public string Username { get; set; }
}