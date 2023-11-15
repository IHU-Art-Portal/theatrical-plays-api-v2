using Theatrical.Data.Models;

namespace Theatrical.Dto.PersonDtos;

public class PersonDto
{
    public int Id { get; set; }
    public string Fullname { get; set; }
    public int? SystemID { get; set; }
    public string? Birthdate { get; set; }
    public string? Bio { get; set; }
    public string? Description { get; set; }
    public List<string>? Languages { get; set; }
    public string? Weight { get; set; }
    public string? Height { get; set; }
    public string? EyeColor { get; set; }
    public string? HairColor { get; set; }
    public List<string>? Roles { get; set; }
    public List<Image>? Images { get; set; }
    public bool IsClaimed { get; set; }
    public string ClaimingStatus { get; set; }
}