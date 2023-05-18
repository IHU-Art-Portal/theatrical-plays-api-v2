namespace Theatrical.Data.Models;

public class Event
{
    public int Id { get; set; }
    public int ProductionId { get; set; }
    public virtual Production Production { get; set; }
    public int VenueId { get; set; }
    public virtual Venue Venue { get; set; }
    public string DateEvent { get; set; }
    public string PriceRage { get; set; }
    public DateTime Created { get; set; }
}