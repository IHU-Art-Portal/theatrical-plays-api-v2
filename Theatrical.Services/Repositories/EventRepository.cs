using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IEventRepository
{
    Task<List<Event>>? Get();
    Task Create(Event newEvent);
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
    
    public async Task Create(Event newEvent)
    {
        await _context.Events.AddAsync(newEvent);
        await _context.SaveChangesAsync();
    }
}

