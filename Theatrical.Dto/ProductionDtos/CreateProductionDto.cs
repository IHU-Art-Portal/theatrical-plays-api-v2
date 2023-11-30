namespace Theatrical.Dto.ProductionDtos;

public class CreateProductionDto
{
    public int OrganizerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string Producer { get; set; }
    public string MediaUrl { get; set; }
    public string? Duration { get; set; }
    public int SystemId { get; set; }
}