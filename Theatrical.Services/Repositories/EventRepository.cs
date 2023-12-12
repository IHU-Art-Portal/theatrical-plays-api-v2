using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Dto.EventDtos;
using Theatrical.Dto.OrganizerDtos;
using Theatrical.Dto.ProductionDtos;
using Theatrical.Dto.ShowsLocal;
using Theatrical.Dto.VenueDtos;
using Theatrical.Services.Caching;

namespace Theatrical.Services.Repositories;

public interface IEventRepository
{
    Task<List<Event>?> Get();
    Task<Event?> GetEvent(int id);
    Task<Event> Create(Event newEvent);
    Task Delete(Event deletingEvent);
    Task UpdatePriceEvent(Event @event, UpdateEventDto eventDto);
    Task<List<Event>> GetEventsForPerson(int personId);
    Task<List<Event>?> GetEventsForProduction(int productionId);
    Task<List<Event>> CreateEvents(List<Event> events);
    Task<List<Show>> GetShows();
    Task<Event> UpdateEvent(Event eventToUpdate);
}

public class EventRepository : IEventRepository
{
    private readonly TheatricalPlaysDbContext _context;
    private readonly ILogRepository _logRepository;
    private readonly ICaching _caching;

    public EventRepository(TheatricalPlaysDbContext context, ILogRepository logRepository, ICaching caching)
    {
        _context = context;
        _logRepository = logRepository;
        _caching = caching;
    }
    
    public async Task<List<Event>> GetEventsForPerson(int personId)
    {
        
        var events = await _caching.GetOrSetAsync($"Events_For_Person_{personId}", async () =>
        {
            return await _context.Persons
                .Where(p => p.Id == personId)
                .SelectMany(p => p.Contributions)
                .SelectMany(c => c.Production.Events)
                .ToListAsync();
        });
        
        return events;
    }

    public async Task<List<Event>?> GetEventsForProduction(int productionId)
    {
        var events = await _caching.GetOrSetAsync($"Events_For_Production_{productionId}", async () =>
        {
            return await _context.Productions
                .Where(p => p.Id == productionId)
                .SelectMany(p => p.Events)
                .ToListAsync();
        });

        return events;
    }

    public async Task<List<Event>> CreateEvents(List<Event> events)
    {
        await _context.Events.AddRangeAsync(events);
        await _context.SaveChangesAsync();
        return events;
    }

    public async Task<List<Show>> GetShows()
    {
        var shows = await _caching.GetOrSetAsyncWithSlidingWindow("All_Shows", async () =>
        {
            //This function returns all events, productions and venues. needs optimizing if caching is not used.
            return await _context.Events
                .Include(e => e.Production)
                .Include(e => e.Venue)
                .Select(e => new Show
                {
                    Production = new ProductionDto
                    {
                        Id = e.Production.Id,
                        OrganizerId = e.Production.OrganizerId,
                        Title = e.Production.Title,
                        Description = e.Production.Description,
                        Url = e.Production.Url,
                        Producer = e.Production.Producer,
                        MediaUrl = e.Production.MediaUrl,
                        Duration = e.Production.Duration
                    },
                    VenueResponseDto = new VenueResponseDto
                    {
                        Id = e.Venue.Id,
                        Address = e.Venue.Address,
                        Title = e.Venue.Title
                    },
                    OrganizerShortenedDto = new OrganizerShortenedDto
                    {
                        Id = e.Production.Organizer!.Id,
                        Email = e.Production.Organizer.Email,
                        Name = e.Production.Organizer.Name,
                        Phone = e.Production.Organizer.Phone
                    },
                    EventDate = e.DateEvent
                }).ToListAsync();
        });

        return shows;
    }

    public async Task<Event> UpdateEvent(Event eventToUpdate)
    {
        _context.Events.Update(eventToUpdate);
        await _context.SaveChangesAsync();
        return eventToUpdate;
    }

    public async Task<List<Event>?> Get()
    {
        return await _caching.GetOrSetAsync("all_events", async () => await _context.Events.ToListAsync());
    }

    public async Task<Event?> GetEvent(int id)
    {
        return await _caching.GetOrSetAsync($"event_{id}", async () => await _context.Events.FindAsync(id));
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

