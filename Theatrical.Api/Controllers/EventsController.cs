using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Theatrical.Data.Models;
using Theatrical.Dto.EventDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository repo;

    public EventsController(IEventRepository repo)
    {
        this.repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<List<Event>>> GetEvents()
    {
        return Ok(await repo.Get());
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
    {
        var eventNew = new Event
        {
            ProductionId = createEventDto.ProductionId,
            VenueId = createEventDto.VenueId,
            DateEvent = createEventDto.DateEvent,
            PriceRage = createEventDto.PriceRange,
            Created = DateTime.UtcNow
        };
        await repo.Create(eventNew);
        return Ok();
    }
}