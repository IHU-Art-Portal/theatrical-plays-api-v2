using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponseContributions
{
    public List<Contribution?> Contributions { get; set; }
    public int CorrectedObjects { get; set; }
    public int OutOf { get; set; }
}