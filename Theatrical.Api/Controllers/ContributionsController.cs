using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.ContributionDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Repositories;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContributionsController : ControllerBase
{
    private readonly IContributionValidationService _validation;
    private readonly IContributionService _service;

    public ContributionsController(IContributionValidationService validation, IContributionService service)
    {
        _service = service;
        _validation = validation;
    }
    
    [HttpGet]
    public async Task<ActionResult<TheatricalResponse>> GetContributions()
    {
        var (validation, contributions) = await _validation.ValidateForFetch();

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message!);
            return new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status404NotFound};
        }

        var response = new TheatricalResponse<List<Contribution>>(contributions, "Completed");
        
        return new OkObjectResult(response);
    }

    [HttpPost]
    public async Task<ActionResult<TheatricalResponse>> CreateContribution([FromBody] CreateContributionDto contributionDto)
    {
        var validation = await _validation.ValidateForCreate(contributionDto);

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message!);
            return new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status404NotFound};
        }

        await _service.Create(contributionDto);
        
        var response = new TheatricalResponse("Successfully Created Contribution");

        return new OkObjectResult(response);
    }
}