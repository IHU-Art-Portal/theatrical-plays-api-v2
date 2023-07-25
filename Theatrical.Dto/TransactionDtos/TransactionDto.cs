namespace Theatrical.Dto.TransactionDtos;

public class TransactionDto
{
    public int UserId { get; set; }
    public decimal CreditAmount { get; set; }
    public string Reason { get; set; }
}