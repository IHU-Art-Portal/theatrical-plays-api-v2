namespace Theatrical.Dto.OrganizerDtos;

public class OrganizersCreationResponseDto
{
    public int UpdatedCount { get; set; }
    public int CreatedCount { get; set; }
    public List<OrganizerDto> OrganizerDtos { get; set; }
}