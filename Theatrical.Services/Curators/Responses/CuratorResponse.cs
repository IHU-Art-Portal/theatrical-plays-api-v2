namespace Theatrical.Services.Curators.Responses;

public class CuratorResponse
{
    public int CorrectedObjects { get; set; }
    public int OutOf { get; set; }

    public CuratorResponse(int correctedObjects, int outOf)
    {
        CorrectedObjects = correctedObjects;
        OutOf = outOf;
    }
}