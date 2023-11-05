namespace Theatrical.Dto.AccountRequestDtos;

public class CreateAccountRequestDto
{
    public int PersonId { get; set; }
    public byte[] IdentificationDocument { get; set; } //byte array of pdf.
}