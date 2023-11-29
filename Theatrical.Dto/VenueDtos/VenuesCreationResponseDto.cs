namespace Theatrical.Dto.VenueDtos;

public class VenuesCreationResponseDto
{
    public int UpdatedCount { get; set; }
    public int CreatedCount { get; set; }
    public List<VenueDto> VenueDtos { get; set; }
}