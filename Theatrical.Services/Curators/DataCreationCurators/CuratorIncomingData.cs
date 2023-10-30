using System.Text.RegularExpressions;

namespace Theatrical.Services.Curators.DataCreationCurators;


public interface ICuratorIncomingData
{
    bool ValidateFullName(string fullName);
}

public class CuratorIncomingData : ICuratorIncomingData
{
    private static readonly Regex FullNameRegex =
        new Regex(@"^[A-ZΑ-Ω][A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff]*(?:[- ][A-ZΑ-Ω][A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff]*)*$");

    public bool ValidateFullName(string fullName)
    {
        return FullNameRegex.IsMatch(fullName);
    }
}