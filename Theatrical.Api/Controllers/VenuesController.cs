using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Dto.VenueDtos;
using Theatrical.Services;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IVenueService _service;
    private readonly IVenueValidationService _validation;

    public VenuesController(IVenueService service, IVenueValidationService validation)
    {
        _service = service;
        _validation = validation;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetVenues()
    {
        try
        {
            var (validation, venues) = await _validation.ValidateAndFetch();

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
                return new NotFoundObjectResult(errorResponse);
            }

            var venuesDto = _service.ToDto(venues!);

            var response = new ApiResponse<List<VenueDto>>(venuesDto);

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message.ToString());

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse>> GetVenue(int id)
    {
        try
        {
            var (validation, venue) = await _validation.ValidateAndFetch(id);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
                return new NotFoundObjectResult(errorResponse);
            }

            var venueDto = _service.ToDto(venue!);

            return new OkObjectResult(venueDto);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message.ToString());

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    [HttpPost]
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> CreateVenue([FromBody] VenueCreateDto venueCreateDto)
    {
        try
        {
            await _service.Create(venueCreateDto);

            var response = new ApiResponse("Venue successfully added");

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message.ToString());

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /*[HttpDelete]
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

    /*[HttpPut]
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