namespace Theatrical.Data.Models;

public class UserImage
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public string ImageLocation { get; set; }
    public string? Label { get; set; }
}