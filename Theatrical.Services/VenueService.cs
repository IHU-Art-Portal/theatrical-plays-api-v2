using Theatrical.Data.Models;
using Theatrical.Dto.VenueDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IVenueService
{
    Task Create(VenueCreateDto venueCreateDto);
    Task Delete(Venue venue);
    Task Update(VenueUpdateDto venue);
    List<VenueDto> ToDto(List<Venue> venue);
    VenueDto ToDto(Venue venue);
}

public class VenueService : IVenueService
{
    private readonly IVenueRepository _repository;

    public VenueService(IVenueRepository repository)
    {
        _repository = repository;
    }
    
    public async Task Create(VenueCreateDto venueCreateDto)
    {
        Venue venue = new Venue
        {
            Title = venueCreateDto.Address,
            Address = venueCreateDto.Address,
            Timestamp = DateTime.UtcNow
        }; 
        
        await _repository.Create(venue);
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
}