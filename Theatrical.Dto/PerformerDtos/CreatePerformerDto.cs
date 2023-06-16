namespace Theatrical.Dto.PerformerDtos;

public class CreatePerformerDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public List<string>? Images { get; set; }
}