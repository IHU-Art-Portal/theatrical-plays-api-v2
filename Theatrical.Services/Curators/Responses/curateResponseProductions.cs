using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class curateResponseProductions
{
    public List<Production?> Productions { get; set; }
    public int CorrectedObjects { get; set; }
    public int OutOf { get; set; }
}