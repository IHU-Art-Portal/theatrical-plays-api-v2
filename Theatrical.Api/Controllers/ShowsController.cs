using System.Globalization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Dto.ShowsLocal;
using Theatrical.Services.Pagination;
using Theatrical.Services.Repositories;

namespace Theatrical.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class ShowsController : ControllerBase
{
    private readonly IEventRepository _repo;
    private readonly IPaginationService _pagination;

    public ShowsController(IEventRepository repository, IPaginationService paginationService)
    {
        _repo = repository;
        _pagination = paginationService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetShows(string? venueAddress, int? venueId, string? eventDate, 
        string? organizerName, string? productionTitle, int page = 1, int size = 10)
    {
        try
        {
            if (eventDate is not null)
            {
                if (!DateTime.TryParseExact(eventDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    return new BadRequestObjectResult(new ApiResponse(ErrorCode.BadRequest, "Wrong date format. Make sure it looks like: dd-MM-yyyy"));
                }
            }
            var shows = await _repo.GetShows();

            if (venueAddress is not null)
            {
                shows = shows
                    .Where(s => s.VenueResponseDto != null && s.VenueResponseDto.Address.Contains(venueAddress, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (venueId is not null)
            {
                shows = shows
                    .Where(s => s.VenueResponseDto != null && s.VenueResponseDto.Id == venueId)
                    .ToList();
            }

            if (eventDate is not null)
            {
                if (DateTime.TryParseExact(eventDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    shows = shows
                        .Where(s => s.EventDate.HasValue && s.EventDate.Value.Date == parsedDate.Date)
                        .ToList();
                }
            }

            if (organizerName is not null)
            {
                shows = shows
                    .Where(s => s.OrganizerShortenedDto.Name.Contains(organizerName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (productionTitle is not null)
            {
                shows = shows
                    .Where(s => s.Production.Title.Contains(productionTitle, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var paginationResult = _pagination.GetPaginated(page, size, shows, items =>
            {
                return items.Select(show => new Show
                {
                    OrganizerShortenedDto = show.OrganizerShortenedDto,
                    VenueResponseDto = show.VenueResponseDto,
                    Production = show.Production,
                    Event = show.Event,
                    EventDate = show.EventDate
                }).ToList();
            });
            
            
            var apiResponse = new ApiResponse<PaginationResult<Show>>(paginationResult);
            return Ok(apiResponse);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
}