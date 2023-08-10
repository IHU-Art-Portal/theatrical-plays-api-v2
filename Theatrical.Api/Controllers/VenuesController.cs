using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Dto.VenueDtos;
using Theatrical.Services;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _service;
    private readonly IVenueValidationService _validation;

    public VenuesController(IVenueService service, IVenueValidationService validation)
    {
        _service = service;
        _validation = validation;
    }

    /// <summary>
    /// Endpoint to all venues.
    /// Customizable with page and size.
    /// </summary>
    /// <param name="page">page number</param>
    /// <param name="size">size capacity for a page</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetVenues(int? page, int? size)
    {
        try
        {
            var (validation, venues) = await _validation.ValidateAndFetch();

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new NotFoundObjectResult(errorResponse);
            }

            var venuesDto = _service.ToDto(venues!);

            var paginationResult = _service.Paginate(page, size, venuesDto);

            var response = new ApiResponse<PaginationResult<VenueDto>>(paginationResult);

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Endpoint to specific Venue.
    /// Search a Venue by its Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse>> GetVenue(int id)
    {
        try
        {
            var (validation, venue) = await _validation.ValidateAndFetch(id);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new NotFoundObjectResult(errorResponse);
            }

            var venueDto = _service.ToDto(venue!);

            return new OkObjectResult(venueDto);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Endpoint to creating a venue.
    /// </summary>
    /// <param name="venueCreateDto"></param>
    /// <returns></returns>
    [HttpPost]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> CreateVenue([FromBody] VenueCreateDto venueCreateDto)
    {
        try
        {
            var createdVenue = await _service.Create(venueCreateDto);

            var createdVenueDto = _service.ToDto(createdVenue);

            var response = new ApiResponse<VenueDto>(createdVenueDto, "Venue successfully added");

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
    
    /*/// <summary>
    /// Endpoint to deleting a venue.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteVenue(int id)
    {
        var (validation, venue) = await _validation.ValidateForDelete(id);

        if (!validation.Success)
        {
            var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }
        
        await _service.Delete(venue);
        
        ApiResponse response = new ApiResponse(message: $"Venue with ID: {id} has been deleted!");
        
        return new ObjectResult(response);
    }*/

    /*/// <summary>
    /// Endpoint to updating a Venue
    /// </summary>
    /// <param name="venueDto"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<ApiResponse>> UpdateVenue([FromBody] VenueUpdateDto venueDto)
    {
        var validation = await _validation.ValidateForUpdate(venueDto);
        
        if (!validation.Success)
        {
            var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }
        
        await _service.Update(venueDto);
        ApiResponse response = new ApiResponse(message: $"Venue with ID: {venueDto.Id} has been updated!");
        
        return new ObjectResult(response);
    }*/

}