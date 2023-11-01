namespace Theatrical.Data.Models;

public class AccountRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PersonId { get; set; }
    public string IdentificationDocument { get; set; }
    public ConfirmationStatus ConfirmationStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual User User { get; set; }
    public virtual Person Person { get; set; }
    public string? AuthorizedBy { get; set; }
}

public enum ConfirmationStatus
{
    Active = 0,
    Approved = 1,
    Rejected = 2
}