using Theatrical.Data.Models;

namespace Theatrical.Dto.AccountRequestDtos;

public class AccountRequestDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PersonId { get; set; }
    public byte[] IdentificationDocument { get; set; }
    public ConfirmationStatus ConfirmationStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}