using System.Text.RegularExpressions;

namespace Theatrical.Services.Curators;

public interface IDataCurator
{
    bool ValidateFullName(string fullName);
    bool ValidateRole(string fullName);
    bool ValidateVenueTitle(string title);
    bool ValidateProductionTitle(string title);
}

public class DataCurator : IDataCurator
{
    private static readonly Regex FullNameRegex =
        new Regex(@"^[A-ZΑ-Ω][A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff]*(?:[- ][A-ZΑ-Ω][A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff]*)*$");

    public bool ValidateFullName(string fullName)
    {
        return FullNameRegex.IsMatch(fullName);
    }

    public bool ValidateRole(string fullName)
    {
        return FullNameRegex.IsMatch(fullName);
    }

    public bool ValidateVenueTitle(string title)
    {
        return FullNameRegex.IsMatch(title);
    }

    public bool ValidateProductionTitle(string title)
    {
        return FullNameRegex.IsMatch(title);
    }
}