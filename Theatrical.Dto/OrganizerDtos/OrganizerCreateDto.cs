namespace Theatrical.Dto.OrganizerDtos;

public class OrganizerCreateDto
{
    public string Name { get; set; }
    public string? Address { get; set; }
    public string? Town { get; set; }
    public string? Postcode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Doy { get; set; }
    public string? Afm { get; set; }
    public int SystemId { get; set; }
}