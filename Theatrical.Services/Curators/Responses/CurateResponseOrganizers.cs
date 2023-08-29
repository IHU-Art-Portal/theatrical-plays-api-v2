using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponseOrganizers
{
    public List<Organizer?> Organizers { get; set; }
    public int CorrectedObjects { get; set; }
    public int OutOf { get; set; }
}