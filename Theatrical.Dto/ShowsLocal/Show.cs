using Theatrical.Data.Models;
using Theatrical.Dto.OrganizerDtos;
using Theatrical.Dto.ProductionDtos;
using Theatrical.Dto.VenueDtos;

namespace Theatrical.Dto.ShowsLocal;

public class Show
{
    public OrganizerShortenedDto? OrganizerShortenedDto { get; set; }
    public VenueResponseDto? VenueResponseDto { get; set; }
    public ProductionDto? Production { get; set; }
    public Event? Event { get; set; }
    public DateTime? EventDate { get; set; }
}