namespace Theatrical.Dto.ProductionDtos;

public class ProductionsCreationResponseDto
{
    public int UpdatedCount { get; set; }
    public int CreatedCount { get; set; }
    public List<ShortenedProductionDto> ShortenedProductionDtos { get; set; }
}