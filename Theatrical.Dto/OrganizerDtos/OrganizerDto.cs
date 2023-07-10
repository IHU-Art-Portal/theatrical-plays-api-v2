namespace Theatrical.Dto.OrganizerDtos;

public class OrganizerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Town { get; set; } = null!;
    public string Postcode { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Doy { get; set; } = null!;
    public string Afm { get; set; } = null!;
    public int SystemId { get; set; }
    public DateTime Timestamp { get; set; }
}