using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponsePeople<T> : CuratorResponse
{
    public T? PeopleCorrected { get; set; }

    public CurateResponsePeople(T? data, int correctedObjects, int outOf) : base(correctedObjects, outOf)
    {
        PeopleCorrected = data;
    }
}