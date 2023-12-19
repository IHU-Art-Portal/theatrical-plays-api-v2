namespace Theatrical.Data.Models;

public class UserEvent
{
    public int Id { get; set; }

    // Foreign key to the Users table
    public int UserId { get; set; }
    // Foreign key to the Events table
    public int EventId { get; set; }
    public DateTime DateCreated { get; set; }
    
    
    //Navigational Properties
    public virtual Event Event { get; set; }
    public virtual User User { get; set; }
}
