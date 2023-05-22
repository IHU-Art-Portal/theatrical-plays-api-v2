using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IEventRepository
{
    Task<List<Event>>? Get();
    Task Create(Event newEvent);
    Task Delete(Event deletingEvent);
}

public class EventRepository : IEventRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public EventRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task<List<Event>>? Get()
    {
        return await _context.Events.ToListAsync();
    }

    public async Task<Event?> GetEvent(int id)
    {
        return await _context.Events.FindAsync(id);
    }
    
    public async Task Create(Event newEvent)
    {
        await _context.Events.AddAsync(newEvent);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Event deletingEvent)
    {
        _context.Events.Remove(deletingEvent);
        await _context.SaveChangesAsync();
    }
}

