using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IEventRepository
{
    Task<List<Event>?> Get();
    Task<Event?> GetEvent(int id);
    Task Create(Event newEvent);
    Task Delete(Event deletingEvent);
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

    public async Task<List<Event>?> Get()
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

        await _logRepository.UpdateLogs("insert", "events", new List<(string ColumnName, string Value)>
        {
            ("ID", newEvent.Id.ToString()),
            ("ProductionID", newEvent.ProductionId.ToString()),
            ("VenueID", newEvent.VenueId.ToString()),
            ("DateEvent", newEvent.DateEvent.ToString(CultureInfo.CurrentCulture)),
            ("PriceRange", newEvent.PriceRange),
            ("SystemId", newEvent.SystemId.ToString())
        });

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
}

