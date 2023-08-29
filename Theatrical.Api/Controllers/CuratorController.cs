using Microsoft.AspNetCore.Mvc;
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

        var curateResponseContributions = new CurateResponseContributions
        {
            Contributions = contributionsProcessed,
            CorrectedObjects = contributionsProcessed.Count,
            OutOf = contributions.Count
        };
        
        var response = new ApiResponse<CurateResponseContributions>(curateResponseContributions);
        
        return Ok(response);
    }

    [HttpGet]
    [Route("curateOrganizersNotSaving")]
    public async Task<ActionResult<ApiResponse>> CurateOrganizers()
    {
        var organizers = await _organizers.Get();

        Curator curator = new Curator();

        var organizersProcessed = curator.CleanData(organizers);

        var curateResponseOrganizers = new CurateResponseOrganizers
        {
            Organizers = organizersProcessed,
            CorrectedObjects = organizersProcessed.Count,
            OutOf = organizers.Count
        };

        var response = new ApiResponse<CurateResponseOrganizers>(curateResponseOrganizers);
        
        return Ok(response);
    }

    [HttpGet]
    [Route("curatePeopleNotSaving")]
    public async Task<ActionResult<ApiResponse>> CurratePeople()
    {
        var people = await _person.Get();

        Curator curator = new Curator();

        var peopleProcessed = curator.CleanData(people);

        var curateResponsePeople = new CurateResponsePeople
        {
            People = peopleProcessed,
            CorrectedObjects = peopleProcessed.Count,
            OutOf = people.Count
        };

        var response = new ApiResponse<CurateResponsePeople>(curateResponsePeople);

        return new OkObjectResult(response);
    }

    [HttpGet]
    [Route("curateProductionsNotSaving")]
    public async Task<ActionResult> CurateProductions()
    {
        var productions = await _production.Get();
        
        Curator curator = new Curator();

        var productionsProcessed = curator.CleanData(productions);

        var curateResponseProductions = new CurateResponseProductions
        {
            Productions = productionsProcessed,
            CorrectedObjects = productionsProcessed.Count,
            OutOf = productions.Count
        };
        
        var response = new ApiResponse<CurateResponseProductions>(curateResponseProductions);

        return new OkObjectResult(response);
    }
    
    [HttpGet]
    [Route("curateRolesNotSaving")]
    public async Task<ActionResult> CurateRoles()
    {
        var roles = await _role.GetRoles();
        
        Curator curator = new Curator();

        var productionsProcessed = curator.CleanData(roles);

        var curateResponseRoles = new CurateResponseRoles
        {
            Roles = productionsProcessed,
            CorrectedObjects = productionsProcessed.Count,
            OutOf = roles.Count
        };
        
        var response = new ApiResponse<CurateResponseRoles>(curateResponseRoles);

        return new OkObjectResult(response);
    }
    
    [HttpGet]
    [Route("curateVenuesNotSaving")]
    public async Task<ActionResult> CurateVenues()
    {
        var venues = await _venues.Get();
        
        Curator curator = new Curator();

        var venuesProcessed = curator.CleanData(venues);

        var curateResponseVenues = new CurateResponseVenues()
        {
            Venues = venuesProcessed,
            CorrectedObjects = venuesProcessed.Count,
            OutOf = venues.Count
        };
        
        var response = new ApiResponse<CurateResponseVenues>(curateResponseVenues);

        return new OkObjectResult(response);
    }
    
}