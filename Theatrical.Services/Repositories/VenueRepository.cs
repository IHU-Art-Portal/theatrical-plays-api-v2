using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IVenueRepository
{
    Task<List<Venue>>? Get();
    Task<Venue?> Get(int id);
    Task Create(Venue venue);
    Task Delete(Venue venue);
    Task Update(Venue venue);
}

public class VenueRepository : IVenueRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public VenueRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task<List<Venue>>? Get()
    {
        var venues = await _context.Venues.ToListAsync();
        return venues;
    }

    public async Task<Venue?> Get(int id)
    {
        var venue = await _context.Venues.FindAsync(id);
        return venue;
    }

    public async Task Create(Venue venue)
    {
        await _context.Venues.AddAsync(venue);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Venue venue)
    {
        _context.Venues.Remove(venue);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Venue venue)
    {
        var venueToUpdate = await _context.Venues.FindAsync(venue.Id);

        venueToUpdate.Address = venue.Address;
        venueToUpdate.Title = venue.Title;
        
        _context.Venues.Update(venueToUpdate);
        await _context.SaveChangesAsync();
        
    }
}