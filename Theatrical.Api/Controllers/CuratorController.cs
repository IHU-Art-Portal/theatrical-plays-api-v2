using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Curators;
using Theatrical.Services.Curators.Responses;
using Theatrical.Services.Repositories;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuratorController : ControllerBase
{
    private readonly IContributionRepository _repo;
    private readonly IOrganizerRepository _organizers;
    private readonly IPersonRepository _person;
    private readonly IProductionRepository _production;
    private readonly IRoleRepository _role;
    private readonly IVenueRepository _venues;

    public CuratorController(IContributionRepository repo, IOrganizerRepository organizerRepository, IPersonRepository personRepository, IProductionRepository productionRepository,
        IRoleRepository roleRepository, IVenueRepository venueRepository)
    {
        _repo = repo;
        _organizers = organizerRepository;
        _person = personRepository;
        _production = productionRepository;
        _role = roleRepository;
        _venues = venueRepository;
    }
    
    [HttpGet]
    [Route("curateContributionsNotSaving")]
    public async Task<ActionResult<ApiResponse>> CurateExistingData()
    {
        var contributions = await _repo.Get();

        if (contributions.Count == 0)
        {
            return new NotFoundObjectResult(new ApiResponse(ErrorCode.NotFound, "Not found any contributions"));
        }
        
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
    }
    
}