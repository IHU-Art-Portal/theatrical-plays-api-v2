using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Identity;
using Theatrical.Data.Models;
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
    public async Task<ActionResult<TheatricalResponse>> GetVenues()
    {
        var (validation, venues) = await _validation.ValidateAndFetch();

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new NotFoundObjectResult(errorResponse);
        }
        var response = new TheatricalResponse<List<Venue>>(venues);
        
        return new ObjectResult(venues);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<TheatricalResponse>> GetVenue(int id)
    {
        var (validation, venue) = await _validation.ValidateAndFetch(id);

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse);
        }

        return new OkObjectResult(venue);
    }

    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpPost]
    public async Task<ActionResult<TheatricalResponse>> CreateVenue([FromBody] VenueCreateDto venueCreateDto)
    {
        
        try
        {
            await _service.Create(venueCreateDto);
        }
        catch (Exception e)
        {
            return new ObjectResult(e.Message);
        }

        var response = new TheatricalResponse("Venue successfully added");

        return new ObjectResult(response);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteVenue(int id)
    {
        var (validation, venue) = await _validation.ValidateForDelete(id);

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }
        
        await _service.Delete(venue);
        
        TheatricalResponse response = new TheatricalResponse(message: $"Venue with ID: {id} has been deleted!");
        
        return Ok();
    }

}