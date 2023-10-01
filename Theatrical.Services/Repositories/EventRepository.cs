using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Dto.EventDtos;

namespace Theatrical.Services.Repositories;

public interface IEventRepository
{
    Task<List<Event>?> Get();
    Task<Event?> GetEvent(int id);
    Task<Event> Create(Event newEvent);
    Task Delete(Event deletingEvent);
    Task UpdatePriceEvent(Event @event, UpdateEventDto eventDto);
    Task<List<Event>> GetEventsForPerson(int personId);
}

public class EventRepository : IEventRepository
{
    private readonly TheatricalPlaysDbContext _context;
    private readonly ILogRepository _logRepository;

    public EventRepository(TheatricalPlaysDbContext context, ILogRepository logRepository)
    {
        _context = context;
        _logRepository = logRepository;
    }
    
    public async Task<List<Event>> GetEventsForPerson(int personId)
    {
        var events = await _context.Persons
            .Where(p => p.Id == personId)
            .SelectMany(p => p.Contributions)
            .Include(c => c.Production)
            .ThenInclude(p => p.Events)
            .ThenInclude(e => e.Venue)
            .SelectMany(c => c.Production.Events)
            .Include(e => e.Production)
            .ToListAsync();
            

        return events;
    }

    public async Task<List<Event>?> Get()
    {
        return await _context.Events.ToListAsync();
    }

    public async Task<Event?> GetEvent(int id)
    {
        return await _context.Events.FindAsync(id);
    }
    
    public async Task<Event> Create(Event newEvent)
    {
        await _context.Events.AddAsync(newEvent);
        await _context.SaveChangesAsync();

        await _logRepository.UpdateLogs("insert", "events", new List<(string ColumnName, string Value)>
        {
            ("ID", newEvent.Id.ToString()),
            ("ProductionID", newEvent.ProductionId.ToString()),
            ("VenueID", newEvent.VenueId.ToString()),
            ("DateEvent", newEvent.DateEvent.ToString(CultureInfo.CurrentCulture)),
            ("PriceRange", newEvent.PriceRange),
            ("SystemId", newEvent.SystemId.ToString())
        });

        return newEvent;

    }

    public async Task Delete(Event deletingEvent)
    {
        _context.Events.Remove(deletingEvent);
        await _context.SaveChangesAsync();
        
        await _logRepository.UpdateLogs("delete", "events", new List<(string ColumnName, string Value)>
        {
            ("ID", deletingEvent.Id.ToString()),
            ("ProductionID", deletingEvent.ProductionId.ToString()),
            ("VenueID", deletingEvent.VenueId.ToString()),
            ("DateEvent", deletingEvent.DateEvent.ToString(CultureInfo.CurrentCulture)),
            ("PriceRange", deletingEvent.PriceRange),
            ("SystemId", deletingEvent.SystemId.ToString())
        });
    }

    public async Task UpdatePriceEvent(Event @event, UpdateEventDto eventDto)
    {
        var oldPriceRange = @event.PriceRange;
        
        @event.PriceRange = eventDto.PriceRange;
        
        await _context.SaveChangesAsync();

        await _logRepository.UpdateLogs("update", "events", new List<(string ColumnName, string Value)>
        {
            ("ID", @event.Id.ToString()),
            ("PriceRange", $"From {oldPriceRange} to {eventDto.PriceRange}")
        });
    }
}

