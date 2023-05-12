using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.PerformersService;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

/// <summary>
/// Not yet implemented
/// </summary>
[ApiController]
[Route("/api/[controller]")]
public class PerformersController : ControllerBase
{
    private readonly IPerformerService _service;
    private readonly IPerformerValidationService _validation;

    public PerformersController(IPerformerService service, IPerformerValidationService validation)
    {
        _service = service;
        _validation = validation;
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
        
        var performerDto = await _service.Get(id);

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
    public async Task<ActionResult> CreatePerformer([FromBody] CreatePerformerDto createPerformerDto)
    {
        await _service.Create(createPerformerDto);
        return Ok();
    }

    /*[HttpGet]
    [Route("role/{value}")]
    public IActionResult GetPerformersRole(string value, int? page, int? size)
    {
        return Ok();
    }*/

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult<TheatricalResponse>> DeletePerformer(int id)
    {
        var (validation, performer) = await _validation.ValidateForDelete(id);

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }
        
        await _service.Delete(performer);

        return NoContent();
    }
}