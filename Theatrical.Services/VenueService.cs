using Theatrical.Data.Models;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.VenueDtos;
using Theatrical.Services.Pagination;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IVenueService
{
    Task<Venue> Create(VenueCreateDto venueCreateDto);
    Task Delete(Venue venue);
    Task Update(VenueUpdateDto venue);
    List<VenueDto> ToDto(List<Venue> venue);
    VenueDto ToDto(Venue venue);
    PaginationResult<VenueDto> Paginate(int? page, int? size, List<VenueDto> venuesDto);
}

public class VenueService : IVenueService
{
    private readonly IVenueRepository _repository;
    private readonly IPaginationService _pagination;

    public VenueService(IVenueRepository repository, IPaginationService paginationService)
    {
        _repository = repository;
        _pagination = paginationService;
    }
    
    public async Task<Venue> Create(VenueCreateDto venueCreateDto)
    {
        Venue venue = new Venue
        {
            Title = venueCreateDto.Address,
            Address = venueCreateDto.Address,
            Timestamp = DateTime.UtcNow
        }; 
        
        var createdVenue = await _repository.Create(venue);
        return createdVenue;
    }

    public async Task Delete(Venue venue)
    {
        await _repository.Delete(venue);
    }

    public async Task Update(VenueUpdateDto venueDto)
    {
        Venue venue = new Venue
        {
            Id = venueDto.Id,
            Title = venueDto.Title,
            Address = venueDto.Address
        };
        await _repository.Update(venue);
    }

    public List<VenueDto> ToDto(List<Venue> venues)
    {
        var venuesDtos = new List<VenueDto>();

        foreach (var venue in venues)
        {
            VenueDto venueDto = new VenueDto
            {
                Id = venue.Id,
                Title = venue.Title,
                Address = venue.Address,
                SystemId = venue.SystemId
            };
            venuesDtos.Add(venueDto);
        }
        
        
        return venuesDtos;
    }

    public VenueDto ToDto(Venue venue)
    {
        var venueDto = new VenueDto
        {
            Id = venue.Id,
            Title = venue.Title,
            Address = venue.Address,
            SystemId = venue.SystemId
        };
        
        return venueDto;
    }

    public PaginationResult<VenueDto> Paginate(int? page, int? size, List<VenueDto> venuesDto)
    {
        var result = _pagination.GetPaginated(page, size, venuesDto, items =>
        {
            return items.Select(venue => new VenueDto
            {
                Address = venue.Address,
                Id = venue.Id,
                SystemId = venue.SystemId,
                Title = venue.Title
            });
        });
        
        return result;
    }
}