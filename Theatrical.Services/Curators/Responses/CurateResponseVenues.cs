using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponseVenues
{
    public List<Venue> Venues { get; set; }
    public int CorrectedObjects { get; set; }
    public int OutOf { get; set; }
}