using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Dto.ResponseWrapperFolder;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PerformersController : ControllerBase
{

    private static List<PerformerDto> AllPerformers = new();
    
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<TheatricalResponse<PerformerDto>>> GetPerformer(int id)
    {
        var performer = AllPerformers.FirstOrDefault(p => p.Id == id);
        TheatricalResponse response = new TheatricalResponse<PerformerDto>(performer);
        
        return new ObjectResult(response);
    }

    [HttpGet]
    public async Task<ActionResult<TheatricalResponse<List<PerformerDto>>>> GetPerformers(int? page, int? size)
    {
        TheatricalResponse response = new TheatricalResponse<List<PerformerDto>>(AllPerformers);
        
        return new ObjectResult(response);
    }

    [HttpPost]
    public ActionResult CreatePerformer([FromBody] PerformerDto performerDto)
    {
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