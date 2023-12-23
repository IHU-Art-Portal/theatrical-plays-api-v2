using Theatrical.Data.Models;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IUserVenueService
{
    Task CreateUserVenue(User user, Venue venue);
    Task<List<UserVenue>?> GetUserVenues(string email);
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
        await _repository.Claim(venue);
        
        var userVenue = new UserVenue
        {
            UserId = user.Id,
            VenueId = venue.Id
        };
        
        await _repository.Create(userVenue);
        
    }

    public async Task<List<UserVenue>?> GetUserVenues(string email)
    {
        var userWithVenues = await _repository.GetUserWithVenues(email);
        return userWithVenues?.UserVenues;
    }
}

