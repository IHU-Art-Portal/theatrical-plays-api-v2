namespace Theatrical.Dto.PersonDtos;

public class CreatePersonDto
{
    public string Fullname { get; set; }
    public List<string>? Images { get; set; }
    public int System { get; set; }
}