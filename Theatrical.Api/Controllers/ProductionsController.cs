using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.LoginDtos;
using Theatrical.Dto.ProductionDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
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
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> CreateProduction([FromBody] CreateProductionDto createProductionDto)
    {
        try
        {
            var validation = await _validation.ValidateForCreate(createProductionDto);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var createdProduction = await _service.Create(createProductionDto);

            var response = new ApiResponse<ProductionDto>(createdProduction, "Successfully Created Production");

            return new ObjectResult(response) { StatusCode = 201 };
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message.ToString());

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetProductions()
    {
        try
        {
            var (validation, productions) = await _validation.ValidateAndFetch();

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var productionsDto = _service.ConvertToDto(productions);

            var response = new ApiResponse<List<ProductionDto>>(productionsDto);

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message.ToString());

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /*[HttpDelete]
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> DeleteProduction(int id)
    {
        var (validation, production) = await _validation.ValidateForDelete(id);

        if (!validation.Success)
        {
            var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }

        await _service.Delete(production);
        var response = new ApiResponse("Successfully Delete Production");

        return new ObjectResult(response);
    }*/
}