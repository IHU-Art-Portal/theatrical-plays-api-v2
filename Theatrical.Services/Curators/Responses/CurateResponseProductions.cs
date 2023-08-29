using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponseProductions
{
    public List<Production?> Productions { get; set; }
    public int CorrectedObjects { get; set; }
    public int OutOf { get; set; }
}