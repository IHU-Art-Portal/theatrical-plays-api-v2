namespace Theatrical.Data.Models;

public class Image
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public int PersonId { get; set; }
    public virtual Person Person { get; set; } //An image belongs to a performer
}