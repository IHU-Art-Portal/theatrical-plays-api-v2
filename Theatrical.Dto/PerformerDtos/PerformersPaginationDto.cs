namespace Theatrical.Dto.PerformerDtos;

public class PerformersPaginationDto
{
    public List<PersonDto>? Persons { get; set; }
    public int? PageSize { get; set; }
    public int? CurrentPage { get; set; }
}