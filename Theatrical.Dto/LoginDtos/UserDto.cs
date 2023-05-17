using System.ComponentModel.DataAnnotations;

namespace Theatrical.Dto.LoginDtos;

public class UserDto
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }
}