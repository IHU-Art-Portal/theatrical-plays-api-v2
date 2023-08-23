using System.ComponentModel.DataAnnotations.Schema;

namespace Theatrical.Data.Models;

public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public decimal CreditAmount { get; set; }
    public string Reason { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime DateCreated { get; set; }
    public long TransactionId { get; set; }
    public string AuthCode { get; set; }
    public string NetworkTransactionId { get; set; }
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
}