using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.ContributionDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Curators;
using Theatrical.Services.Curators.Responses;
using Theatrical.Services.Repositories;
using Theatrical.Dto.RoleDtos;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class CuratorController : ControllerBase
{
    private readonly IContributionRepository _contributions;
    private readonly IOrganizerRepository _organizers;
    private readonly IPersonRepository _person;
    private readonly IProductionRepository _production;
    private readonly IRoleRepository _role;
    private readonly IVenueRepository _venues;
    private readonly IRoleSimplifierCurator _roleSimplifierCurator;

    public CuratorController(IContributionRepository contributions, IOrganizerRepository organizerRepository, IPersonRepository personRepository, IProductionRepository productionRepository,
        IRoleRepository roleRepository, IVenueRepository venueRepository, IRoleSimplifierCurator roleSimplifierCurator)
    {
        _contributions = contributions;
        _organizers = organizerRepository;
        _person = personRepository;
        _production = productionRepository;
        _role = roleRepository;
        _venues = venueRepository;
        _roleSimplifierCurator = roleSimplifierCurator;
    }

    /// <summary>
    /// Removes special html symbols, redundant spaces and some useless symbols.
    /// </summary>
    /// <returns></returns>
    [HttpGet("CurateSimple")]
    public async Task<ActionResult<ApiResponse>> CurateAll()
    {
        try
        {
            var contributions = await _contributions.Get();
            var organizers = await _organizers.Get();
            var people = await _person.Get();
            var productions = await _production.Get();
            var roles = await _role.GetRoles();
            var venues = await _venues.Get();

            Curator curator = new Curator();

            var contributionsProcessed = curator.CleanData(contributions);
            
            var organizersProcessed = curator.CleanData(organizers);
            
            var peopleProcessed = curator.CleanData(people);

            var productionsProcessed = curator.CleanData(productions);

            var rolesProcessed = curator.CleanData(roles);

            var venuesProcessed = curator.CleanData(venues);
            
            var curateResponse = new CurateResponseEverything
            {
                ContributionsCorrected = contributionsProcessed.Count,
                OrganizersCorrected = organizersProcessed.Count,
                PeopleCorrected = peopleProcessed.Count,
                ProductionsCorrected = productionsProcessed.Count,
                RolesCorrected = rolesProcessed.Count,
                VenuesCorrected = venuesProcessed.Count
            };

            await _contributions.UpdateRange(contributionsProcessed);
            await _organizers.UpdateRange(organizersProcessed);
            await _person.UpdateRange(peopleProcessed);
            await _production.UpdateRange(productionsProcessed);
            await _role.UpdateRange(rolesProcessed);
            await _venues.UpdateRange(venuesProcessed);

            var apiresponse = new ApiResponse<CurateResponseEverything>(curateResponse);

            return new OkObjectResult(apiresponse);
        }
        catch (Exception e)
        {
            return new ObjectResult(e.Message);
        }
    }

    [HttpGet]
    [Route("Contributions/Delete/ForNonExistent/People")]
    public async Task<ActionResult<ApiResponse>> CorrectContr()
    {
        var contributions = await _contributions.Get();
        var persons = await _person.Get();

        var contributionsToDelete = contributions
            .Where(contribution => !persons.Any(person => person.Id == contribution.PeopleId))
            .ToList();

        var contrResp = new DeletedContributionsDto
        {
            DeletedCount = contributionsToDelete.Count,
            TotalContributions = contributions.Count
        };

        await _contributions.RemoveRange(contributionsToDelete);
        
        var apiresponse = new ApiResponse<DeletedContributionsDto>(contrResp);
        
        return new OkObjectResult(apiresponse);
    }

    [HttpGet]
    [Route("Contributions/Delete/ForNonExistent/Productions")]
    public async Task<ActionResult<ApiResponse>> CorrectContrProd()
    {
        var contributions = await _contributions.Get();
        var productions = await _production.Get();

        var contributionsToDelete = contributions
            .Where(contribution => productions.All(production => production.Id != contribution.ProductionId))
            .ToList();
        
        var contrResp = new DeletedContributionsDto
        {
            DeletedCount = contributionsToDelete.Count,
            TotalContributions = contributions.Count
        };

        await _contributions.RemoveRange(contributionsToDelete);
        
        var apiresponse = new ApiResponse<DeletedContributionsDto>(contrResp);
        
        return new OkObjectResult(apiresponse);
    }

    [HttpGet]
    [Route("Roles/Simplify/LevenshteinDistance")]
    public async Task<ActionResult<ApiResponse>> FindSimilarRoles()
    {
        List<Role> roles = await _role.GetRoles();
        
        //Similar roles list.
        List<Role> similarRoles = _roleSimplifierCurator.FindSimilarRoles(roles);

        if (similarRoles.Count == 0)
        {
            return new ObjectResult(new ApiResponse("The list of similar roles is empty. Thus there is no need for role correction."));
        }

        //Custom dictionary with mapped roles.
        Dictionary<string, string> dictionary = _roleSimplifierCurator.GetCorrectedRoleDictionary();

        //Sends the list of roles for correction along with the dictionary.
        //This list will be used to update these entries in db.
        List<Role> correctedRoles = _roleSimplifierCurator.MapWrongRolesToCorrect(similarRoles, dictionary);

        correctedRoles = correctedRoles.OrderBy(role => role.Id).ToList();
        correctedRoles = correctedRoles.DistinctBy(role => role.Id).ToList();
        
        var response = new CuratorRoleResponse
        {
            CorrectedRoles = correctedRoles.Count,
            roles = correctedRoles,
            TotalRoles = roles.Count
        };

        return Ok(response);
    }
    
    /*
    [HttpGet]
    [Route("curateContributionsNotSaving")]
    public async Task<ActionResult<ApiResponse>> CurateContributions()
    {
        var contributions = await _repo.Get();

        Curator curator = new Curator();
        
        var contributionsProcessed = curator.CleanData(contributions);

        var curateResponseContributions = new CurateResponseContributions<List<Contribution>>(contributionsProcessed, contributionsProcessed.Count, contributions.Count);
        
        var response = new ApiResponse<CurateResponseContributions<List<Contribution>>>(curateResponseContributions);
        
        return Ok(response);
    }

    [HttpGet]
    [Route("curateOrganizersNotSaving")]
    public async Task<ActionResult<ApiResponse>> CurateOrganizers()
    {
        var organizers = await _organizers.Get();

        Curator curator = new Curator();

        var organizersProcessed = curator.CleanData(organizers);

        var curateResponseOrganizers = new CurateResponseOrganizers<List<Organizer>>(organizersProcessed, organizersProcessed.Count, organizers.Count);

        var response = new ApiResponse<CurateResponseOrganizers<List<Organizer>>>(curateResponseOrganizers);
        
        return Ok(response);
    }

    [HttpGet]
    [Route("curatePeopleNotSaving")]
    public async Task<ActionResult<ApiResponse>> CurratePeople()
    {
        var people = await _person.Get();

        Curator curator = new Curator();

        var peopleProcessed = curator.CleanData(people);

        var curateResponsePeople = new CurateResponsePeople<List<Person>>(peopleProcessed, peopleProcessed.Count, people.Count);

        var response = new ApiResponse<CurateResponsePeople<List<Person>>>(curateResponsePeople);

        return new OkObjectResult(response);
    }

    [HttpGet]
    [Route("curateProductionsNotSaving")]
    public async Task<ActionResult> CurateProductions()
    {
        var productions = await _production.Get();
        
        Curator curator = new Curator();

        var productionsProcessed = curator.CleanData(productions);

        var curateResponseProductions =
            new CurateResponseProductions<List<Production>>(productionsProcessed, productionsProcessed.Count,
                productions.Count);
        
        var response = new ApiResponse<CurateResponseProductions<List<Production>>>(curateResponseProductions);

        return new OkObjectResult(response);
    }
    
    [HttpGet]
    [Route("curateRolesNotSaving")]
    public async Task<ActionResult> CurateRoles()
    {
        var roles = await _role.GetRoles();
        
        Curator curator = new Curator();

        var rolesProcessed = curator.CleanData(roles);

        var curateResponseRoles = new CurateResponseRoles<List<Role>>(rolesProcessed, rolesProcessed.Count, roles.Count);
        
        var response = new ApiResponse<CurateResponseRoles<List<Role>>>(curateResponseRoles);

        return new OkObjectResult(response);
    }
    
    [HttpGet]
    [Route("curateVenuesNotSaving")]
    public async Task<ActionResult> CurateVenues()
    {
        var venues = await _venues.Get();
        
        Curator curator = new Curator();

        var venuesProcessed = curator.CleanData(venues);
        
        var curateResponseVenues = new CurateResponseVenues<List<Venue>>(venuesProcessed, venuesProcessed.Count, venues.Count);
        
        var response = new ApiResponse<CurateResponseVenues<List<Venue>>>(curateResponseVenues);

        return new OkObjectResult(response);
    }*/
    
}