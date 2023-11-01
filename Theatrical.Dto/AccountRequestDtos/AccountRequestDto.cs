namespace Theatrical.Dto.AccountRequestDtos;

public class AccountRequestDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PersonId { get; set; }
    public string IdentificationDocument { get; set; }
    public string ConfirmationStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AuthorizedBy { get; set; }
}