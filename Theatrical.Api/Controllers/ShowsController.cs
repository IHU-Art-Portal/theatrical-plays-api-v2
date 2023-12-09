using System.Globalization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Dto.ShowsLocal;
using Theatrical.Services;
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
    private readonly IShowService _showService;

    public ShowsController(IEventRepository repository, IPaginationService paginationService, IShowService showService)
    {
        _repo = repository;
        _pagination = paginationService;
        _showService = showService;
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

            var filteredShows = _showService.ShowsFiltering(shows, venueAddress, venueId, eventDate, organizerName, productionTitle);

            var paginationResult = _showService.PaginateShows(page, size, filteredShows);
            
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