using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IUserEventRepository
{
    Task<List<UserEvent>> GetUserEventsAsync();
    Task<List<Event>?> GetClaimedEventsByUser(int userId);
    Task<User?> GetUserWithEvents(string email);
    Task Claim(Event @event);
    Task Create(UserEvent userEvent);
}
public class UserEventRepository : IUserEventRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public UserEventRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<UserEvent>> GetUserEventsAsync()
    {
        return await _context.UserEvents.ToListAsync();
    }
    
    public async Task<User?> GetUserWithEvents(string email)
    {
        return await _context.Users
            .Where(u => u.Email == email)
            .Include(u => u.UserEvents)
            .ThenInclude(uv => uv.Event)
            .FirstOrDefaultAsync();
    }

    public async Task Claim(Event @event)
    {
        @event.IsClaimed = true;
        await _context.SaveChangesAsync();
    }

    public async Task Create(UserEvent userEvent)
    {
        await _context.UserEvents.AddAsync(userEvent);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Event>?> GetClaimedEventsByUser(int userId)
    {
        var userEvents = await _context.UserEvents.Where(ue => ue.UserId == userId).ToListAsync();

        var eventIds = userEvents.Select(ue => ue.EventId).ToList();

        var claimedEvents = await _context.Events
            .Where(e => eventIds.Contains(e.Id))
            .ToListAsync();

        return claimedEvents;
    }
}

