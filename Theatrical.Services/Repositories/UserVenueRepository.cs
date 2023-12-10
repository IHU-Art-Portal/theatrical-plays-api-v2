using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IUserVenueRepository
{
    Task<User?> GetUserWithVenues(string email);
    Task Create(UserVenue userVenue);
    Task Claim(Venue venue);
    Task<List<Venue>?> GetClaimedVenuesForUser(int userId);
}

public class UserVenueRepository : IUserVenueRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public UserVenueRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserWithVenues(string email)
    {
        return await _context.Users
            .Where(u => u.Email == email)
            .Include(u => u.UserVenues)
            .ThenInclude(uv => uv.Venue)
            .FirstOrDefaultAsync();
    }

    public async Task Create(UserVenue userVenue)
    {
        await _context.UserVenues.AddAsync(userVenue);
        await _context.SaveChangesAsync();
    }

    public async Task Claim(Venue venue)
    {
        venue.isClaimed = true;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Venue>?> GetClaimedVenuesForUser(int userId)
    {
        var userVenues = await _context.UserVenues.Where(uv => uv.UserId == userId).ToListAsync();

        var venueIds = userVenues.Select(uv => uv.VenueId).ToList();

        var claimedVenues = await _context.Venues
            .Where(v => venueIds.Contains(v.Id))
            .ToListAsync();

        return claimedVenues;
    }
}