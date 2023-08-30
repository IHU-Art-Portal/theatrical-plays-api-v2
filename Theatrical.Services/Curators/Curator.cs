using System.Collections;
using System.Net;
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
                return (T)(object)correctedContributions;
            
            case "Event":
                var correctedEvents = CleanEventData((data as List<Event>)!);
                return (T)(object)correctedEvents;
            
            case "Organizer":
                var correctedOrganizers = CleanOrganizerData((data as List<Organizer>)!);
                return (T)(object)correctedOrganizers;
                
            case "Production":
                var correctedProductions = CleanProductionData((data as List<Production>)!);
                return (T)(object)correctedProductions;
            
            case "Role":
                var correctedRoles = CleanRoleData((data as List<Role>)!);
                return (T)(object)correctedRoles;
            
            case "Person":
                var correctedPeople = CleanPersonData((data as List<Person>)!);
                return (T)(object)correctedPeople;
            
            case "Venue":
                var correctedVenues = CleanVenueData((data as List<Venue>)!);
                return (T)(object)correctedVenues;
            
            default:
                throw new ArgumentException($"Unsupported type: {typeName}");
        }
        
        
    }

    private List<Venue> CleanVenueData(List<Venue> venues)
    {
        List<Venue> venuesProcessed = new();
        
        foreach (var venue in venues)
        {
            if (!string.IsNullOrEmpty(venue.Title))
            {
                if (venue.Title.Contains('\"'))
                {
                    venue.Title = venue.Title.Replace("\"", "");
                    venuesProcessed.Add(venue);
                }

                if (Regex.IsMatch(venue.Title, @"\s{2,}"))
                {
                    venue.Title = Regex.Replace(venue.Title, @"\s{2,}", " ");
                    if (!venuesProcessed.Contains(venue))
                    {
                        venuesProcessed.Add(venue);
                    }
                }
            }

            if (!string.IsNullOrEmpty(venue.Address))
            {
                if (venue.Address.Contains('\"'))
                {
                    venue.Address = venue.Address.Replace("\"", "");
                    if (!venuesProcessed.Contains(venue))
                    {
                        venuesProcessed.Add(venue);
                    }
                }
                
                if (Regex.IsMatch(venue.Address, @"\s{2,}"))
                {
                    venue.Address = Regex.Replace(venue.Address, @"\s{2,}", " ");
                    if (!venuesProcessed.Contains(venue))
                    {
                        venuesProcessed.Add(venue);
                    }
                }
            }
        }
        
        Console.WriteLine($"Processed venues: {venues.Count} and produced: {venuesProcessed.Count}");
        
        return venuesProcessed;
    }

    /// <summary>
    /// Cleans roles data.
    /// </summary>
    /// <param name="roles"></param>
    /// <returns></returns>
    private List<Role> CleanRoleData(List<Role> roles)
    {
        List<Role> rolesProcessed = new();

        foreach (var role in roles)
        {
            //Almost everything here is clean.
            if (!string.IsNullOrEmpty(role.Role1))
            {
                if (Regex.IsMatch(role.Role1, @"\s{2,}"))
                {
                    role.Role1 = Regex.Replace(role.Role1, @"\s{2,}", " ");
                    rolesProcessed.Add(role);
                }
                role.Role1 = role.Role1.Trim();
            }
        }
        
        Console.WriteLine($"Processed roles: {roles.Count} and produced: {rolesProcessed.Count}");
        
        return rolesProcessed;
    }

    /// <summary>
    /// Optimised code for clean the person's fullname field of extra spaces.
    /// </summary>
    /// <param name="people"></param>
    /// <returns></returns>
    private List<Person> CleanPersonData(List<Person> people)
    {
        List<Person> peopleProcessed = new();

        foreach (var person in people)
        {
            if (!string.IsNullOrEmpty(person.Fullname))
            {
                if (Regex.IsMatch(person.Fullname, @"\s{2,}"))
                {
                    person.Fullname = Regex.Replace(person.Fullname, @"\s+", " ");

                    person.Fullname = person.Fullname.Trim();
                    peopleProcessed.Add(person);
                }
            }
        }
        Console.WriteLine($"Processed people: {people.Count}, and produced: {peopleProcessed.Count}");

        return peopleProcessed;
    }

    private List<Production> CleanProductionData(List<Production> productions)
    {
        List<Production> productionsProcessed = new();

        //Productions
        foreach (var production in productions)
        {
            if (Regex.IsMatch(production.Description, @"\s{2,}"))
            {
                production.Producer = Regex.Replace(production.Producer, @"\s{2,}", "");
                SaveProduction(production, productionsProcessed);
            }
            
            //Description
            if (!string.IsNullOrEmpty(production.Description))
            {

                if (Regex.IsMatch(production.Description, @"\r|\n"))
                {
                    production.Description = production.Description.Replace("\r", " ").Replace("\n", " ");
                    SaveProduction(production, productionsProcessed);
                }

                //Special case: removes stars from production with id: 802
                if (production.Description.Contains("*"))
                {
                    production.Description = Regex.Replace(production.Description, @"\*", "");
                    production.Description = Regex.Replace(production.Description, @"\s{2,}", " ");
                    SaveProduction(production, productionsProcessed);
                }

                if (production.Description.Contains('\"'))
                {
                    production.Description = production.Description.Replace("\"", "");
                    SaveProduction(production, productionsProcessed);
                }

                //Special case: removes \\ from production with id: 1348
                if (production.Description.Contains('\\'))
                {
                    production.Description = production.Description.Replace("\\", "");
                }

                //Special case: removes ''
                if (production.Description.Contains("''"))
                {
                    production.Description = production.Description.Replace("''", "");
                }

                //Last but foremost!
                //Removes double or more spaces.
                if (Regex.IsMatch(production.Description, @"\s{2,}"))
                {
                    production.Description = Regex.Replace(production.Description, @"\s{2,}", " ");
                }

                production.Description = production.Description.Trim();
            }

            //Producer
            if (!string.IsNullOrEmpty(production.Producer))
            {
                //Removes \r and \n
                if (Regex.IsMatch(production.Producer, @"\r|\n"))
                {
                    production.Producer = production.Producer.Replace("\r", " ").Replace("\n", " ");
                    SaveProduction(production, productionsProcessed);
                }
                
                //Removes \"
                if (production.Producer.Contains('\"'))
                {
                    production.Producer = production.Producer.Replace("\"", "");
                    SaveProduction(production, productionsProcessed);
                }
                
                //Removes multiple spaces.
                if (Regex.IsMatch(production.Producer, @"\s{2,}"))
                {
                    production.Producer = Regex.Replace(production.Producer, @"\s{2,}", "");
                    SaveProduction(production, productionsProcessed);
                }
            }

            
            //Title
            if (!string.IsNullOrEmpty(production.Title))
            {

                if (Regex.IsMatch(production.Title, @""""))
                {
                    production.Title = production.Title.Replace(@"""", "").Replace(@"\\", "");
                    SaveProduction(production, productionsProcessed);
                }

                if (Regex.IsMatch(production.Title, @"\s{2,}"))
                {
                    production.Title = Regex.Replace(production.Title, @"\s{2,}", " ");
                    SaveProduction(production, productionsProcessed);
                }
                
                //Removes double ' from title.
                //Special case for id: 1045
                if (production.Title.Contains("''"))
                {
                    production.Title = production.Title.Replace("''", "");
                    SaveProduction(production, productionsProcessed);
                }

                production.Title = production.Title.Trim();
            }
        }

        Console.WriteLine($"Processed productions: {productions.Count} and produced: {productionsProcessed.Count}");
        
        return productionsProcessed;
    }

    private void SaveProduction(Production? productionToBeSaved, List<Production?> productions)
    {
        if (!productions.Contains(productionToBeSaved))
        {
            productions.Add(productionToBeSaved);
        }
    }

    private List<Organizer> CleanOrganizerData(List<Organizer> organizers)
    {
        List<Organizer> organizersProcessed = new();

        foreach (var organizer in organizers)
        {
            if (!string.IsNullOrEmpty(organizer.Name))
            {
                
                if (Regex.IsMatch(organizer.Name, @"\r|\n"))
                {
                    organizer.Name = organizer.Name.Replace("\r", "").Replace("\n", "");
                    organizer.Name = organizer.Name.Trim();
                    organizersProcessed.Add(organizer);
                }
                
                string pattern = @"[^\p{L}\p{N}\s]";
                if (Regex.IsMatch(organizer.Name, pattern))
                {
                    organizer.Name = Regex.Replace(organizer.Name, pattern, "");
                    organizer.Name = organizer.Name.Trim();
                    if (!organizersProcessed.Contains(organizer))
                    {
                        organizersProcessed.Add(organizer);
                    }
                }

                if (Regex.IsMatch(organizer.Name, @"\s{2,}"))
                {
                    organizer.Name = Regex.Replace(organizer.Name, @"\s{2,}", " ");
                    organizer.Name = organizer.Name.Trim();
                    if (!organizersProcessed.Contains(organizer))
                    {
                        organizersProcessed.Add(organizer);
                    }
                }
            }

            if (!string.IsNullOrEmpty(organizer.Address))
            {
                if (Regex.IsMatch(organizer.Address, @"\s{2,}"))
                {
                    organizer.Address = Regex.Replace(organizer.Address, @"\s{2,}", " ");
                    organizer.Address = organizer.Address.Trim();
                    
                    if (!organizersProcessed.Contains(organizer))
                    {
                        organizersProcessed.Add(organizer);
                    }
                }
            }

            if (!string.IsNullOrEmpty(organizer.Email))
            {
                if (Regex.IsMatch(organizer.Email, @"\r|\n"))
                {
                    organizer.Email = organizer.Email.Replace("\r", "").Replace("\n", "");
                    
                    if (!organizersProcessed.Contains(organizer))
                    {
                        organizersProcessed.Add(organizer);
                    }
                }
            }
        }
        
        Console.WriteLine($"Processed organizers: {organizers.Count} and produced: {organizersProcessed.Count}");

        return organizersProcessed;
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
                if (Regex.IsMatch(contribution.SubRole, pattern))
                {
                    //Special symbols removal, like \ and " or -.
                    contribution.SubRole = Regex.Replace(contribution.SubRole, pattern, "");
                    
                    //Adds to the processed list.
                    contributionsProcessed.Add(contribution);
                }

                if (Regex.IsMatch(contribution.SubRole, @"\s{2,}"))
                {
                    // Redundant spaces removal.
                    contribution.SubRole = Regex.Replace(contribution.SubRole, @"\s+", " ");
                    contribution.SubRole = contribution.SubRole.Trim();
                    
                    //Adds to the processed list only if it does not exist.
                    if (!contributionsProcessed.Contains(contribution))
                    {
                        contributionsProcessed.Add(contribution);
                    }
                }
            }
        }
        
        Console.WriteLine($"Processed contributions: {contributions.Count} and produced: {contributionsProcessed.Count}");

        return contributionsProcessed;
    }
}