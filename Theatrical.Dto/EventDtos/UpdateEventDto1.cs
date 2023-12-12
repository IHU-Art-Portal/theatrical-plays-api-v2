namespace Theatrical.Dto.EventDtos;

public class UpdateEventDto1
{
    public int EventId { get; set; }
    public string? PriceRange { get; set; }
    public string? EventDate { get; set; } = null!;
    public int? ProductionId { get; set; }
    public int? VenueId { get; set; }
    public int SystemId { get; set; }
}