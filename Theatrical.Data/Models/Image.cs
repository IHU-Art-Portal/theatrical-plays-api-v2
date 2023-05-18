namespace Theatrical.Data.Models;

public class Image
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public int PerformerId { get; set; }
    public virtual Performer Performer { get; set; } //An image belongs to a performer
}