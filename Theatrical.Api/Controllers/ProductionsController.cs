using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Identity;
using Theatrical.Data.Models;
using Theatrical.Dto.ProductionDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Repositories;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionsController : ControllerBase
{
    private readonly IProductionValidationService _validation;
    private readonly IProductionService _service;

    public ProductionsController(IProductionService service, IProductionValidationService validation)
    {
        _service = service;
        _validation = validation;
    }
    
    [HttpPost]
    public async Task<ActionResult<TheatricalResponse>> CreateProduction([FromBody] CreateProductionDto createProductionDto)
    {
        var validation = await _validation.ValidateForCreate(createProductionDto);

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse) { StatusCode = 404 };
        }

        var createdProduction = await _service.Create(createProductionDto);

        var response = new TheatricalResponse<ProductionDto>(createdProduction, "Successfully Created Production");

        return new ObjectResult(response){StatusCode = 201};
    }

    [HttpGet]
    public async Task<ActionResult<TheatricalResponse>> GetProductions()
    {
        var (validation, productions) = await _validation.ValidateAndFetch();

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }

        var productionsDto = _service.ConvertToDto(productions);

        var response = new TheatricalResponse<List<ProductionDto>>(productionsDto);
        
        return new OkObjectResult(response);
    }

    [Authorize(Policy = IdentityData.AdminUserPolicyName)]
    [HttpDelete]
    public async Task<ActionResult<TheatricalResponse>> DeleteProduction(int id)
    {
        var (validation, production) = await _validation.ValidateForDelete(id);

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }

        await _service.Delete(production);
        var response = new TheatricalResponse("Successfully Delete Production");

        return new ObjectResult(response);
    }
}