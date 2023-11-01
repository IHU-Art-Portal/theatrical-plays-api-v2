namespace Theatrical.Dto.AccountRequestDtos;

public class CreateAccountRequestDto
{
    public int PersonId { get; set; }
    public string IdentificationDocument { get; set; } //imageBase64
}