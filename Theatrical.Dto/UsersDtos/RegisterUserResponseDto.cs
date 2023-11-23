namespace Theatrical.Dto.UsersDtos;

public class RegisterUserResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public bool Enabled { get; set; }
    public string? Note { get; set; }
}