using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.EventDtos;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
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

            var eventDtos = _service.ToDto(events);

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
    
    /// <summary>
    /// Endpoint to creating a new Event.
    /// </summary>
    /// <param name="createEventDto"></param>
    /// <returns></returns>
    [HttpPost]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
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

            await _service.Create(createEventDto);
            var response = new ApiResponse("Successfully Created Event");

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
    
}