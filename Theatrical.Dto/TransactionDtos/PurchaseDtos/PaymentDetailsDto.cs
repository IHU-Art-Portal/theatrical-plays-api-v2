namespace Theatrical.Dto.TransactionDtos.PurchaseDtos;

public class PaymentDetailsDto
{
    public int UserId { get; set; }
    public decimal CreditAmount { get; set; }
    public string Reason { get; set; }
    public long TransactionId { get; set; }
    public string AuthCode { get; set; }
    public string NetworkTransactionId { get; set; }
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
}