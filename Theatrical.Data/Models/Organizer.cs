namespace Theatrical.Data.Models;

public class Organizer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Address { get; set; }
    public string? Town { get; set; }
    public int? Postcode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Doy { get; set; }
    public string? Afm { get; set; }
    public DateTime Created { get; set; }
    public virtual List<Production> Productions { get; set; } //One too many, an organizer can have many productions
}