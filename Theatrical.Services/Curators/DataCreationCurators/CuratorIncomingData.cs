using System.Globalization;
using System.Text.RegularExpressions;

namespace Theatrical.Services.Curators.DataCreationCurators;


public interface ICuratorIncomingData
{
    string CorrectFullName(string fullName);
    List<string> CorrectRolesOrLanguages(List<string> roles);
}

public class CuratorIncomingData : ICuratorIncomingData
{
    private static readonly Regex FullNameRegex =
        new Regex(@"^[A-ZΑ-Ω][A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff]*(?:[- ][A-ZΑ-Ω][A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff]*)*$");

    public string CorrectFullName(string fullName)
    {
        if (!FullNameRegex.IsMatch(fullName))
        {
            fullName = fullName.Trim();
            // Remove non-alphabetical characters and correct the name
            string correctedName = Regex.Replace(fullName, @"[^A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff\s,]", "");

            // Remove extra spaces and commas and ensure the corrected name matches the pattern
            correctedName = Regex.Replace(correctedName, @"\s*,\s*", ", ");

            correctedName = ToTitleCase(correctedName);

            return correctedName;
        }

        return fullName;
    }
    
    public List<string> CorrectRolesOrLanguages(List<string> roles)
    {
        var correctedRoles = new List<string>();
        
        foreach (var role in roles)
        {
            var correctRole = new string(role.Trim());
            if (!FullNameRegex.IsMatch(role))
            {
                correctRole = Regex.Replace(correctRole, @"[^A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff\s,]", "");
                correctRole = Regex.Replace(correctRole,@"\s*,\s*", ", " );
                correctRole = ToTitleCase(correctRole);
                correctRole = RemoveExtraSpaces(correctRole);
                correctedRoles.Add(correctRole);
            }
        }
        
        
        return correctedRoles;
    }
    
    private string RemoveExtraSpaces(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        // Use regular expression to replace multiple spaces with a single space
        return Regex.Replace(input, @"\s+", " ");
    }


    private string ToTitleCase(string text)
    {
        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(text.ToLower());
    }
}