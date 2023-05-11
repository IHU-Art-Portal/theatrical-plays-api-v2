using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Dto.ResponseWrapperFolder;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PerformersController : ControllerBase
{

    public static List<PerformerDto> AllPerformers = new();
    
    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetPerformer(int id)
    {
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<TheatricalResponse<List<PerformerDto>>>> GetPerformers(int? page, int? size)
    {
        TheatricalResponse response = new TheatricalResponse<List<PerformerDto>>(AllPerformers);
        
        return new ObjectResult(response);
    }

    [HttpPost]
    public ActionResult CreatePerformer([FromBody] CreatePerformerDto createPerformerDto)
    {
        PerformerDto performerDto = new PerformerDto
        {
            FullName = createPerformerDto.FullName,
            Image = createPerformerDto.Image
        };
        
        AllPerformers.Add(performerDto);
        return Ok();
    }

    [HttpGet]
    [Route("role/{value}")]
    public IActionResult GetPerformersRole(string value, int? page, int? size)
    {
        return Ok();
    }
}