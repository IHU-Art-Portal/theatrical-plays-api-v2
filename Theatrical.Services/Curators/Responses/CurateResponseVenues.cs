using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponseVenues<T> : CuratorResponse
{
    public T? CorrectedVenues { get; }

    public CurateResponseVenues(T? data, int correctedVenues, int outOf) : base(correctedVenues, outOf)
    {
        CorrectedVenues = data;
    }
}