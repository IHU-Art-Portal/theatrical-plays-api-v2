namespace Theatrical.Data.Models;

public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public decimal CreditAmount { get; set; }
    public string Reason { get; set; }
    public DateTime DateCreated { get; set; }
}