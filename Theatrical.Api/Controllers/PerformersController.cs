using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.PerformersService;

namespace Theatrical.Api.Controllers;

/// <summary>
/// Not yet implemented
/// </summary>
[ApiController]
[Route("/api/[controller]")]
public class PerformersController : ControllerBase
{
    private readonly IPerformerService _service;

    public PerformersController(IPerformerService service)
    {
        _service = service;
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
        return Ok();
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
        //_service pagination logic. here//
        PerformersPaginationDto performersDto = await _service.Get();
        
        TheatricalResponse response = new TheatricalResponse<PerformersPaginationDto>(performersDto);
        
        return new ObjectResult(response);
    }

    [HttpPost]
    public ActionResult CreatePerformer([FromBody] PerformerDto performerDto)
    {
        
        return Ok();
    }

    [HttpGet]
    [Route("role/{value}")]
    public IActionResult GetPerformersRole(string value, int? page, int? size)
    {
        return Ok();
    }
}