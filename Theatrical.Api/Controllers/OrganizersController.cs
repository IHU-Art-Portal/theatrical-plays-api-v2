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
    public async Task<ActionResult<TheatricalResponse>> CreateOrganizer([FromBody] OrganizerCreateDto organizerCreateDto)
    {
        await _service.Create(organizerCreateDto);

        var response = new TheatricalResponse("Successfully created Organizer");
        
        return new OkObjectResult(response);
    }

    [HttpGet]
    public async Task<ActionResult<TheatricalResponse>> GetOrganizers()
    {
        var (report, organizers) = await _validation.ValidateAndFetch();

        if (!report.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, report.Message);
            return new NotFoundObjectResult(errorResponse);
        }
        
        var response = new TheatricalResponse<List<Organizer>>(organizers);
        
        return new ObjectResult(response);
    }
    
    [HttpDelete]
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    [Route("{id}")]
    public async Task<ActionResult<TheatricalResponse>> DeleteOrganizer(int id)
    {
        
        var (report, organizer) = await _validation.ValidateForDelete(id);

        if (!report.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, report.Message);
            return new NotFoundObjectResult(errorResponse);
        }

        await _service.Delete(organizer);
        var response = new TheatricalResponse<Organizer>(organizer, message: "Organizer has been deleted");
        return new OkObjectResult(response);
    }
}