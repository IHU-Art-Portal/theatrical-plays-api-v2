namespace Theatrical.Services.Curators.Responses;

public class CurateResponseOrganizers<T> : CuratorResponse
{
    public T? OrganizersCorrected { get; set; }

    public CurateResponseOrganizers(T? data, int correctedObjects, int outOf) : base(correctedObjects, outOf)
    {
        OrganizersCorrected = data;
    }
}