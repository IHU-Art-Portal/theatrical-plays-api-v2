using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ProductionDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Security.AuthorizationFilters;
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
    
    /// <summary>
    /// Endpoint to creating a new Production.
    /// </summary>
    /// <param name="createProductionDto"></param>
    /// <returns></returns>
    [HttpPost]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProductionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse>> CreateProduction([FromBody] CreateProductionDto createProductionDto)
    {
        try
        {
            var validation = await _validation.ValidateForCreate(createProductionDto);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var createdProduction = await _service.Create(createProductionDto);

            var response = new ApiResponse<ProductionDto>(createdProduction, "Successfully Created Production");

            return new ObjectResult(response) { StatusCode = 201 };
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Endpoint to fetching all productions
    /// Pagination Available.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(PaginationResult<ProductionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetProductions(int? page, int? size)
    {
        try
        {
            var (validation, productions) = await _validation.ValidateAndFetch();

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var productionsDto = _service.ConvertToDto(productions);

            var paginationResult = _service.Paginate(page, size, productionsDto);

            var response = new ApiResponse<PaginationResult<ProductionDto>>(paginationResult);

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    
    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProductionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetProduction([FromRoute] int id)
    {
        try
        {
            var (validation, production) = await _validation.ValidateAndFetchProduction(id);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new NotFoundObjectResult(errorResponse);
            }

            var productionDto = _service.ToDto(production!);
            
            var response = new ApiResponse<ProductionDto>(productionDto);
            
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

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