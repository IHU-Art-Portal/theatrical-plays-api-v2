using Theatrical.Data.Models;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IUserVenueService
{
    Task CreateUserVenue(User user, Venue venue);
}

public class UserVenueService : IUserVenueService
{
    private readonly IUserVenueRepository _repository;

    public UserVenueService(IUserVenueRepository userVenueRepository)
    {
        _repository = userVenueRepository;
    }

    public async Task CreateUserVenue(User user, Venue venue)
    {
        var userVenue = new UserVenue
        {
            UserId = user.Id,
            VenueId = venue.Id
        };
        
        await _repository.Claim(venue);
        await _repository.Create(userVenue);
    }
}

