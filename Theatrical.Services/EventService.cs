using Theatrical.Data.Models;
using Theatrical.Dto.EventDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IEventService
{
    Task<List<EventDto>?> Get();
    Task Create(CreateEventDto createEventDto);
}

public class EventService : IEventService
{
    private readonly IEventRepository _repository;

    public EventService(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EventDto>?> Get()
    {
        var events = await _repository.Get();
        var eventsDto = new List<EventDto>();

        foreach (var newEvent in events)
        {
            var tempEvent = new EventDto
            {
                DateEvent = newEvent.DateEvent,
                PriceRange = newEvent.PriceRange,
                ProductionId = newEvent.ProductionId,
                VenueId = newEvent.VenueId
            };
            eventsDto.Add(tempEvent);
        }

        return eventsDto;
    }

    public async Task Create(CreateEventDto createEventDto)
    {
        var eventNew = new Event
        {
            ProductionId = createEventDto.ProductionId,
            VenueId = createEventDto.VenueId,
            DateEvent = createEventDto.DateEvent,
            PriceRange = createEventDto.PriceRange,
            Timestamp = DateTime.UtcNow
        };
        
        await _repository.Create(eventNew);
    }
}

