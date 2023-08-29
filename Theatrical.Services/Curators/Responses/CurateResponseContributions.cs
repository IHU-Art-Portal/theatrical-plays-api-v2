using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponseContributions<T> : CuratorResponse
{
    public T? ContributionsCorrected { get; set; }

    public CurateResponseContributions(T? data, int correctedObjects, int outOf) : base(correctedObjects, outOf)
    {
        ContributionsCorrected = data;
    }
}