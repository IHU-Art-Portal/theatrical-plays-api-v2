using System.Collections;
using System.Text.RegularExpressions;
using Theatrical.Data.Models;

namespace Theatrical.Services.Curators;

public class Curator
{
    public T CleanData<T>(T data)
    {
        string typeName = null!;
        
        foreach (var variable in (IEnumerable)data!)
        {
            typeName = variable.GetType().Name;
            break;
        }
        Console.WriteLine("Passed: " +typeName);
        
        
        switch (typeName)
        {
            case "Contribution":
                List<Contribution> correctedContributions = CleanContributionData((data as List<Contribution>)!);
                Console.WriteLine($"Corrected Contributions: {correctedContributions.Count}");
                return (T)(object)correctedContributions;
            
            case "Event":
                var correctedEvents = CleanEventData((data as List<Event>)!);
                Console.WriteLine("Corrected Events");
                return (T)(object)correctedEvents;
            
            case "Organizer":
                var correctedOrganizers = CleanOrganizerData((data as List<Organizer>)!);
                Console.WriteLine("Corrected Organizers");
                return (T)(object)correctedOrganizers;
                
            case "Production":
                var correctedProductions = CleanProductionData((data as List<Production>)!);
                Console.WriteLine("Corrected Productions");
                return (T)(object)correctedProductions;
            
            case "Role":
                var correctedRoles = CleanRoleData((data as List<Role>)!);
                Console.WriteLine("Corrected Roles");
                return (T)(object)correctedRoles;
            
            case "Person":
                var correctedPeople = CleanPersonData((data as List<Person>)!);
                Console.WriteLine("Corrected People");
                return (T)(object)correctedPeople;
            
            case "Venue":
                var correctedVenues = CleanVenueData((data as List<Venue>)!);
                Console.WriteLine("Corrected Venues");
                return (T)(object)correctedVenues;
            
            default:
                throw new ArgumentException($"Unsupported type: {typeName}");
        }

        throw new Exception("Invalid Exception");
    }

    private List<Venue> CleanVenueData(List<Venue> data)
    {
        throw new NotImplementedException();
    }

    private List<Role> CleanRoleData(List<Role> data)
    {
        throw new NotImplementedException();
    }

    private List<Person> CleanPersonData(List<Person> data)
    {
        throw new NotImplementedException();
    }

    private List<Production> CleanProductionData(List<Production> data)
    {
        throw new NotImplementedException();
    }

    private List<Organizer> CleanOrganizerData(List<Organizer> data)
    {
        throw new NotImplementedException();
    }

    private List<Event> CleanEventData(List<Event> data)
    {
        throw new NotImplementedException();
    }

    
    // Curation for Contributions
    private List<Contribution> CleanContributionData(List<Contribution> contributions)
    {
        List<Contribution> contributionsProcessed = new();

        foreach (var contribution in contributions)
        {
            //Curates only for non empty/null values
            if (!string.IsNullOrEmpty(contribution.SubRole))
            {
                string pattern = @"[^\p{L}\p{N}\s]";

                // Redundant spaces removal.
                contribution.SubRole = Regex.Replace(contribution.SubRole, @"\s+", " ");
                contribution.SubRole = contribution.SubRole.Trim();
                
                //Special symbols removal, like \ and " or -.
                if (Regex.IsMatch(contribution.SubRole, pattern))
                {
                    contribution.SubRole = Regex.Replace(contribution.SubRole, @"[^\p{L}\p{N}\s]", "");
                    
                }
                //Adds to the list of affected contributions. To be send for update.
                contributionsProcessed.Add(contribution);
            }
        }

        return contributionsProcessed;
    }
}