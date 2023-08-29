using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponsePeople
{
    public List<Person?> People { get; set; }
    public int CorrectedObjects { get; set; }
    public int OutOf { get; set; }
}