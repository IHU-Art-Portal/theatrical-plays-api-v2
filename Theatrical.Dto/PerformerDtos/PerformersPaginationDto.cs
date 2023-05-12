namespace Theatrical.Dto.PerformerDtos;

public class PerformersPaginationDto
{
    public List<PerformerDto> Performers { get; set; }
    public int? PageSize { get; set; }
    public int? CurrentPage { get; set; }
}