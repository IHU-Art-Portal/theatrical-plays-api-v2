namespace Theatrical.Dto.TransactionDtos.PurchaseDtos;

public class TransactionResponseDto
{
    public int UserId { get; set; }
    public int DatabaseTransactionId { get; set; }
    public decimal CreditAmount { get; set; }
    public string Reason { get; set; }
    public DateTime DateCreated { get; set; }
}