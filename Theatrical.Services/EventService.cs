using System.Globalization;
using Theatrical.Data.Models;
using Theatrical.Dto.EventDtos;
using Theatrical.Dto.Pagination;
using Theatrical.Services.Pagination;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IEventService
{
    Task<Event> Create(CreateEventDto createEventDto);
    List<EventDto> ToDtoRange(List<Event> events);
    PaginationResult<EventDto> Paginate(int? page, int? size, List<EventDto> eventDtos);
    Task UpdatePriceRange(Event @event, UpdateEventDto eventDto);
    EventDto ToDto(Event newEvent);
    Task<List<Event>> CreateRange(List<CreateEventDto> createEventDtos);
}

public class EventService : IEventService
{
    private readonly IEventRepository _repository;
    private readonly IPaginationService _pagination;

    public EventService(IEventRepository repository, IPaginationService paginationService)
    {
        _repository = repository;
        _pagination = paginationService;
    }

  
    public List<EventDto> ToDtoRange(List<Event> events)
    {
        return events.Select(event1 => new EventDto
        {
            PriceRange = event1.PriceRange,
            DateEvent = event1.DateEvent,
            ProductionId = event1.ProductionId,
            VenueId = event1.VenueId
        }).ToList();
    }

    public EventDto ToDto(Event newEvent)
    {
        return new EventDto
        {
            PriceRange = newEvent.PriceRange,
            DateEvent = newEvent.DateEvent,
            ProductionId = newEvent.ProductionId,
            VenueId = newEvent.VenueId
        };
    }

    public async Task<List<Event>> CreateRange(List<CreateEventDto> createEventDtos)
    {
        var createEvents = createEventDtos.Select(dto => new Event
        {
            DateEvent = DateTime.SpecifyKind(DateTime.ParseExact(dto.DateEvent, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture), DateTimeKind.Utc),
            PriceRange = dto.PriceRange,
            ProductionId = dto.ProductionId,
            VenueId = dto.VenueId,
            SystemId = dto.SystemId
        }).ToList();
        
        var events = await _repository.CreateEvents(createEvents);
        return events;
    }

    public async Task<Event> Create(CreateEventDto createEventDto)
    {
        var eventNew = new Event
        {
            ProductionId = createEventDto.ProductionId,
            VenueId = createEventDto.VenueId,
            DateEvent = DateTime.SpecifyKind(DateTime.ParseExact(createEventDto.DateEvent, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture), DateTimeKind.Utc),
            PriceRange = createEventDto.PriceRange,
            Timestamp = DateTime.UtcNow,
            SystemId = createEventDto.SystemId
        };
        
        var newEvent = await _repository.Create(eventNew);
        return newEvent;
    }
    
    public PaginationResult<EventDto> Paginate(int? page, int? size, List<EventDto> eventDtos)
    {
        var paginationResult = _pagination.GetPaginated(page, size, eventDtos, items =>
        {
            return items.Select(ev => new EventDto
            {
                ProductionId = ev.ProductionId,
                DateEvent = ev.DateEvent,
                PriceRange = ev.PriceRange,
                VenueId = ev.VenueId
            });
        });

        return paginationResult;
    }

    public async Task UpdatePriceRange(Event @event, UpdateEventDto eventDto)
    {
        await _repository.UpdatePriceEvent(@event, eventDto);
    }
}

