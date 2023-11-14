using System.Globalization;
using System.Text.RegularExpressions;
using Theatrical.Dto.PersonDtos;

namespace Theatrical.Services.Curators.DataCreationCurators;


public interface ICuratorIncomingData
{
    void CorrectIncomingPerson(CreatePersonDto createPersonDto);
    public string? CorrectString(string value);
}

public class CuratorIncomingData : ICuratorIncomingData
{
    private static readonly Regex FullNameRegex =
        new Regex(@"^[A-ZΑ-Ω][A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff]*(?:[- ][A-ZΑ-Ω][A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff]*)*$");

    public void CorrectIncomingPerson(CreatePersonDto createPersonDto)
    {
        //Nullify Empty------------------------------------------------------------------
        createPersonDto.HairColor = NullifyEmptyStrings(createPersonDto.HairColor);
        createPersonDto.Height = NullifyEmptyStrings(createPersonDto.Height);
        createPersonDto.Weight = NullifyEmptyStrings(createPersonDto.Weight);
        createPersonDto.Description = NullifyEmptyStrings(createPersonDto.Description);
        createPersonDto.Bio = NullifyEmptyStrings(createPersonDto.Bio);
        createPersonDto.Birthdate = NullifyEmptyStrings(createPersonDto.Birthdate);
        createPersonDto.EyeColor = NullifyEmptyStrings(createPersonDto.EyeColor);
        createPersonDto.Languages = NullifyEmptyLists(createPersonDto.Languages);
        createPersonDto.Roles = NullifyEmptyLists(createPersonDto.Roles);
        
        //Correct------------------------------------------------------------------
        createPersonDto.HairColor = CorrectString(createPersonDto.HairColor);
        createPersonDto.EyeColor = CorrectString(createPersonDto.EyeColor);
        createPersonDto.Weight = CorrectStringKeepNumbers(createPersonDto.Weight);
        createPersonDto.Description = CorrectStringKeepNumbers(createPersonDto.Description);
        createPersonDto.Bio = CorrectStringKeepNumbers(createPersonDto.Bio);
        //Height case americanized style e.g(5'9"). Keeps quotation marks.
        createPersonDto.Height = CorrectStringKeepNumbersAndQuotationMarks(createPersonDto.Height);
        createPersonDto.Roles = CorrectRolesOrLanguages(createPersonDto.Roles);
        createPersonDto.Languages = CorrectRolesOrLanguages(createPersonDto.Languages);

    }
    
    public string? CorrectString(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        
        if (!FullNameRegex.IsMatch(value))
        {
            value = value.Trim();
            // Remove non-alphabetical characters and correct the name
            string correctedName = Regex.Replace(value, @"[^A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff\s,]", "");

            // Remove extra spaces and commas and ensure the corrected name matches the pattern
            correctedName = Regex.Replace(correctedName, @"\s*,\s*", ", ");

            correctedName = ToTitleCase(correctedName);

            correctedName = RemoveExtraSpaces(correctedName);

            return correctedName;
        }
        

        return value;
    }
    
    private string? CorrectStringKeepNumbers(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        
        if (!FullNameRegex.IsMatch(value))
        {
            value = value.Trim();
            // Remove non-alphabetical characters but keeps the numbers
            string correctedValue = Regex.Replace(value, @"[^0-9A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff\s,]", "");

            // Remove extra spaces and commas and ensure the corrected name matches the pattern
            correctedValue = Regex.Replace(correctedValue, @"\s*,\s*", ", ");

            correctedValue = RemoveExtraSpaces(correctedValue);

            return correctedValue;
        }
        

        return value;
    }

    private string? CorrectStringKeepNumbersAndQuotationMarks(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;
        
        if (!FullNameRegex.IsMatch(value))
        {
            value = value.Trim();
            // Remove non-alphabetical characters but keeps the numbers and ' "
            string correctedValue = Regex.Replace(value, @"[^\dA-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff\s,'""]", "");

            // Remove extra spaces and commas and ensure the corrected name matches the pattern
            correctedValue = Regex.Replace(correctedValue, @"\s*,\s*", ", ");

            correctedValue = RemoveExtraSpaces(correctedValue);

            return correctedValue;
        }

        return value;
    }
    
    private List<string>? CorrectRolesOrLanguages(List<string>? incomingList)
    {
        if (incomingList is null || incomingList.Count == 0)
        {
            return null;
        }

        var correctedRoles = new List<string>();
        
        foreach (var role in incomingList)
        {
            var correctRole = new string(role.Trim());
            if (!FullNameRegex.IsMatch(role))
            {
                correctRole = Regex.Replace(correctRole, @"[^A-Za-zΑ-Ωα-ω\u0370-\u03ff\u1f00-\u1fff\s,]", "");
                correctRole = Regex.Replace(correctRole,@"\s*,\s*", ", " );
                correctRole = ToTitleCase(correctRole);
                correctRole = RemoveExtraSpaces(correctRole);
            }
            correctedRoles.Add(correctRole);
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
    
    
    private string? NullifyEmptyStrings(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        return text;
    }
    
    private List<T>? NullifyEmptyLists<T>(List<T>? list)
    {
        if (list == null || list.Count == 0)
        {
            return null;
        }

        return list;
    }
}