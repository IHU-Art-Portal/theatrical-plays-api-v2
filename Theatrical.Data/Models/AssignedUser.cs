namespace Theatrical.Data.Models;

public class AssignedUser
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PersonId { get; set; }
    public int RequestId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    //Navigational Properties
    public virtual User User { get; set; }
    public virtual Person Person { get; set; }
    public virtual AccountRequest AccountRequest { get; set; }
}