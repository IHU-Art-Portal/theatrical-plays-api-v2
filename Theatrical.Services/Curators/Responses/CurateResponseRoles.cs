using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponseRoles<T> : CuratorResponse
{
    public T? RolesCorrected { get; }


    public CurateResponseRoles(T? data, int correctedObjects, int outOf) : base(correctedObjects, outOf)
    {
        RolesCorrected = data;
    }
}