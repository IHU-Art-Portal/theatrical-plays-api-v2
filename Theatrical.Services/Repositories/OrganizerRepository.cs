using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Services.Caching;

namespace Theatrical.Services.Repositories;

public interface IOrganizerRepository
{
    Task<Organizer?> Get(int id);
    Task<List<Organizer>?> Get();
    Task Create(Organizer organizer);
    Task Delete(Organizer organizer);
    Task<List<Organizer>> UpdateRange(List<Organizer> organizers);
    Task<List<Organizer>> GetOrganizersByNames(List<string> organizersNames);
    Task<List<Organizer>> CreateRange(List<Organizer> organizers);
}

public class OrganizerRepository : IOrganizerRepository
{
    private readonly TheatricalPlaysDbContext _context;
    private readonly ICaching _caching;

    public OrganizerRepository(TheatricalPlaysDbContext context, ICaching caching)
    {
        _context = context;
        _caching = caching;
    }
    
    public async Task<Organizer?> Get(int id)
    {
        var organizer = await _context.Organizers.FindAsync(id);
        return organizer;
    }

    public async Task<List<Organizer>?> Get()
    {
        var organizers = await _caching.GetOrSetAsync("all_organizers", async () => await _context.Organizers.ToListAsync());
        return organizers;
    }

    public async Task Create(Organizer organizer)
    {
        await _context.Organizers.AddAsync(organizer);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Organizer organizer)
    {
        _context.Organizers.Remove(organizer);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Organizer>> UpdateRange(List<Organizer> organizers)
    {
        _context.Organizers.UpdateRange(organizers);
        await _context.SaveChangesAsync();
        return organizers;
    }

    public async Task<List<Organizer>> CreateRange(List<Organizer> organizers)
    {
        await _context.Organizers.AddRangeAsync(organizers);
        await _context.SaveChangesAsync();
        return organizers;
    }

    public async Task<List<Organizer>> GetOrganizersByNames(List<string> organizersNames)
    {
        return await _context.Organizers.Where(o => organizersNames.Contains(o.Name)).ToListAsync();
    }
}

