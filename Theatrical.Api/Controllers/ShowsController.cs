using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
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
    public async Task<ActionResult<ApiResponse>> GetShows(int page = 1, int size = 10)
    {
        try
        {
            var shows = await _repo.GetShows();

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