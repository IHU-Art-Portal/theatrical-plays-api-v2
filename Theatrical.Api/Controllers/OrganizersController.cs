using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;
using Theatrical.Dto.OrganizerDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizersController : ControllerBase
{
    private readonly IOrganizerService _service;
    private readonly IOrganizerValidationService _validation;
    
    public OrganizersController(IOrganizerService service, IOrganizerValidationService validation)
    {
        _service = service;
        _validation = validation;
    }
    
    [HttpPost]
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> CreateOrganizer([FromBody] OrganizerCreateDto organizerCreateDto)
    {
        try
        {
            await _service.Create(organizerCreateDto);

            var response = new ApiResponse("Successfully created Organizer");

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message.ToString());

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetOrganizers()
    {
        try
        {
            var (report, organizers) = await _validation.ValidateAndFetch();

            if (!report.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, report.Message);
                return new NotFoundObjectResult(errorResponse);
            }

            var response = new ApiResponse<List<Organizer>>(organizers);

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message.ToString());

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
    
    /*[HttpDelete]
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteOrganizer(int id)
    {
        
        var (report, organizer) = await _validation.ValidateForDelete(id);

        if (!report.Success)
        {
            var errorResponse = new ApiResponse(ErrorCode.NotFound, report.Message);
            return new NotFoundObjectResult(errorResponse);
        }

        await _service.Delete(organizer);
        var response = new ApiResponse<Organizer>(organizer, message: "Organizer has been deleted");
        return new OkObjectResult(response);
    }*/
}