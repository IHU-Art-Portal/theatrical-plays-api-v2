using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponse
{
    public List<Contribution> Contributions { get; set; }
    public int CorrectedObjects { get; set; }
}