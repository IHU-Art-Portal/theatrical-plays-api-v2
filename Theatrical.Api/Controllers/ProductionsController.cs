using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.ProductionDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionsController : ControllerBase
{
    private readonly IProductionRepository _repo;

    public ProductionsController(IProductionRepository repo)
    {
        _repo = repo;
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateProduction([FromBody] CreateProductionDto createProductionDto)
    {
        var production = new Production
        {
            OrganizerId = createProductionDto.OrganizerId,
            Title = createProductionDto.Title,
            Description = createProductionDto.Description,
            Url = createProductionDto.Url,
            Producer = createProductionDto.Producer,
            MediaUrl = createProductionDto.MediaUrl,
            Duration = createProductionDto.Duration,
            Created = DateTime.UtcNow
        };
        await _repo.Create(production);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult> GetProductions()
    {
        var s = await _repo.Get();
        return Ok(s);
    }
}