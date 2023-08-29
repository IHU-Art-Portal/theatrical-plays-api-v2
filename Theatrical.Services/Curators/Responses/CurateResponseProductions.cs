using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponseProductions<T> : CuratorResponse
{
    public T? ProductionsCorrected { get; }
    public bool CarefullyCrafted { get; }

    public CurateResponseProductions(T? productionsCorrected, int correctedObjects, int outOf) : base(correctedObjects, outOf)
    {
        ProductionsCorrected = productionsCorrected;
        CarefullyCrafted = true;
    }
}