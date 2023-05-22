using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Theatrical.Data.Models;
using Theatrical.Dto.EventDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Repositories;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository repo;
    private readonly IEventValidationService _validation;

    public EventsController(IEventRepository repo, IEventValidationService validation)
    {
        this.repo = repo;
        _validation = validation;
    }

    [HttpGet]
    public async Task<ActionResult<List<Event>>> GetEvents()
    {
        return Ok(await repo.Get());
    }
    
    [HttpPost]
    public async Task<ActionResult<TheatricalResponse>> CreateEvent([FromBody] CreateEventDto createEventDto)
    {
        var validation = await _validation.ValidateForCreate(createEventDto);

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse) { StatusCode = 404 };
        }
        
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