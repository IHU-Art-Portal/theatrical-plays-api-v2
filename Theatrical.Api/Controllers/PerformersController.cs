using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.LoginDtos;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.PerformersService;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PerformersController : ControllerBase
{
    private readonly IPerformerService _service;
    private readonly IPerformerValidationService _validation;
    private readonly IUserValidationService _userValidation;

    public PerformersController(IPerformerService service, IPerformerValidationService validation, IUserValidationService userValidation)
    {
        _service = service;
        _validation = validation;
        _userValidation = userValidation;
    }

    /// <summary>
    /// Retrieves performer information by their Id
    /// </summary>
    /// <param name="id">int</param>
    /// <returns>TheatricalResponse&lt;PerformerDto&gt; object containing performer data.</returns>
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<TheatricalResponse<PerformerDto>>> GetPerformer(int id)
    {
        var (validation, performer) = await _validation.ValidateAndFetch(id);

        if (!validation.Success)
        {
            TheatricalResponse errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }
        
        var performerDto = await _service.Get(performer);

        TheatricalResponse response = new TheatricalResponse<PerformerDto>(performerDto);
        
        return new ObjectResult(response);
    }

    /// <summary>
    /// Retrieves all performers if pagination parameters are not specified
    /// </summary>
    /// <param name="page">Optional. The page number for pagination</param>
    /// <param name="size">Optional. THe page size for pagination</param>
    /// <returns>TheatricalResponse&lt;PerformersPaginationDto&gt; object containing paginated items.</returns>
    [HttpGet]
    public async Task<ActionResult<TheatricalResponse<PerformersPaginationDto>>> GetPerformers(int? page, int? size)
    {

        PerformersPaginationDto performersDto = await _service.Get(page, size);
        
        TheatricalResponse response = new TheatricalResponse<PerformersPaginationDto>(performersDto);
        
        return new ObjectResult(response);
    }

    [HttpPost]
    public async Task<ActionResult<TheatricalResponse>> CreatePerformer([FromBody] CreatePerformerDto createPerformerDto, [FromHeader] string? jwtToken)
    {
        var userValidation = _userValidation.ValidateUser(jwtToken);
        
        if (!userValidation.Success)
        {
            var responseError = new UserErrorMessage(userValidation.Message!).ConstructActionResult();
            return responseError;
        }
        
        await _service.Create(createPerformerDto);

        var response = new TheatricalResponse("Successfully Created Performer");
        
        return new OkObjectResult(response);
    }

    [HttpGet]
    [Route("search")]
    public ActionResult GetPerformersRole(string role, int? page, int? size)
    {
        return StatusCode((int)HttpStatusCode.NotImplemented, "This function is not implemented yet and might be subject to changes.");
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult<TheatricalResponse>> DeletePerformer(int id, [FromHeader] string? jwtToken)
    {
        var userValidation = _userValidation.ValidateUser(jwtToken);
        
        if (!userValidation.Success)
        {
            var responseError = new UserErrorMessage(userValidation.Message!).ConstructActionResult();
            return responseError;
        }
        
        var (validation, performer) = await _validation.ValidateForDelete(id);

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }
        
        await _service.Delete(performer);
        TheatricalResponse response = new TheatricalResponse(message: $"Performer with ID: {id} has been deleted!");
        
        return new OkObjectResult(response);
    }
}