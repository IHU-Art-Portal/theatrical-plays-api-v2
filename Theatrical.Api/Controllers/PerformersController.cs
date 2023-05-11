using Microsoft.AspNetCore.Mvc;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PerformersController : ControllerBase
{

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetPerformer(int id)
    {
        return Ok();
    }

    [HttpGet]
    public IActionResult GetPerformers(int? page, int? size)
    {
        return Ok();
    }
}