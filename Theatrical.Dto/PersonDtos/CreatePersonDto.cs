namespace Theatrical.Dto.PersonDtos;

public class CreatePersonDto
{
    public string Fullname { get; set; }
    public string? HairColor { get; set; }
    public string? Height { get; set; }
    public string? EyeColor { get; set; }
    public string? Weight { get; set; }
    public List<string>? Languages { get; set; }
    public string? Role { get; set; }
    public string? Description { get; set; }
    public string? Bio { get; set; }
    public string? Birthdate { get; set; }
    public List<string>? Images { get; set; }
    public int System { get; set; }
}