using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.EventDtos;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Security.AuthorizationFilters;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/events")]
[EnableCors("AllowOrigin")]
public class EventsController : ControllerBase
{
    private readonly IEventValidationService _validation;
    private readonly IEventService _service;

    public EventsController(IEventService service, IEventValidationService validation)
    {
        _service = service;
        _validation = validation;
    }

    /// <summary>
    /// Endpoint to fetching all Event(s).
    /// Pagination Available.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(PaginationResult<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetEvents(int? page, int? size)
    {
        try
        {
            var (validation, events) = await _validation.FetchAndValidate();

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var eventDtos = _service.ToDtoRange(events);

            var paginationResult = _service.Paginate(page, size, eventDtos);
            
            var response = new ApiResponse<PaginationResult<EventDto>>(paginationResult);
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    [HttpGet]
    [Route("person/{id:int}")]
    [ProducesResponseType(typeof(PaginationResult<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetEventsForPerson([FromRoute] int id, int? page, int? size)
    {
        try
        {
            var (validation, ev) = await _validation.FetchEventsForPerson(id);
            if (!validation.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)validation.ErrorCode!, validation.Message! );
                return new ObjectResult(errorResponse){StatusCode = 404};
            }

            var eventsDto = _service.ToDtoRange(ev);
            
            var paginationResult = _service.Paginate(page, size, eventsDto);

            var response = new ApiResponse<PaginationResult<EventDto>>(paginationResult);
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse(ErrorCode.ServerError, e.Message);
            return new ObjectResult(exceptionResponse);
        }
    }
    
    [HttpGet]
    [Route("production/{id:int}")]
    [ProducesResponseType(typeof(PaginationResult<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetEventsForProduction([FromRoute] int id, int? page, int? size)
    {
        try
        {
            var (validation, ev) = await _validation.FetchEventsForProduction(id);
            if (!validation.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)validation.ErrorCode!, validation.Message! );
                return new ObjectResult(errorResponse){StatusCode = 404};
            }

            var eventsDto = _service.ToDtoRange(ev);
            
            var paginationResult = _service.Paginate(page, size, eventsDto);

            var response = new ApiResponse<PaginationResult<EventDto>>(paginationResult);
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse(ErrorCode.ServerError, e.Message);
            return new ObjectResult(exceptionResponse);
        }
    }
    
    /// <summary>
    /// Endpoint to creating a new Event.
    /// </summary>
    /// <param name="createEventDto"></param>
    /// <returns></returns>
    [HttpPost]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> CreateEvent([FromBody] CreateEventDto createEventDto)
    {
        try
        {
            var validation = await _validation.ValidateForCreate(createEventDto);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var newEvent = await _service.Create(createEventDto);
            var newEventDto = _service.ToDto(newEvent);
            var response = new ApiResponse<EventDto>(newEventDto,"Successfully Created Event");

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    [HttpPost]
    [Route("range")]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    [ProducesResponseType(typeof(List<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> CreateEvents([FromBody] List<CreateEventDto> createEventDtos)
    {
        try
        {
            var createdEvents = await _service.CreateRange(createEventDtos);
            var createdEventsDto = _service.ToDtoRange(createdEvents);

            var response = new ApiResponse<List<EventDto>>(createdEventsDto);
            
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    [HttpPut("update/price")]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> UpdateEventPrice([FromBody] UpdateEventDto eventDto)
    {
        var (validation, @event) = await _validation.ValidateForFetch(eventDto.EventId);

        if (!validation.Success)
        {
            var errorResponse = new ApiResponse((ErrorCode) validation.ErrorCode!, validation.Message!);
            return new ObjectResult(errorResponse) { StatusCode = (int)HttpStatusCode.NotFound };
        }

        await _service.UpdatePriceRange(@event!, eventDto);

        var response = new ApiResponse("Successfully updated event's price!");

        return new OkObjectResult(response);
    }
    
}