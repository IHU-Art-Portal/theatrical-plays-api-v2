using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.enums;
using Theatrical.Dto.ContributionDtos;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Security.AuthorizationFilters;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class ContributionsController : ControllerBase
{
    private readonly IContributionValidationService _validation;
    private readonly IContributionService _service;

    public ContributionsController(IContributionValidationService validation, IContributionService service)
    {
        _service = service;
        _validation = validation;
    }
    
    /// <summary>
    /// Endpoint to fetching all Contribution(s).
    /// Pagination Available.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(PaginationResult<ContributionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetContributions(int? page, int? size)
    {
        try
        {
            var (validation, contributions) = await _validation.ValidateForFetch();

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status404NotFound };
            }

            var contributionDtos = _service.ToDtoRange(contributions);

            var paginationResult = _service.Paginate(page, size, contributionDtos);
            
            var response = new ApiResponse<PaginationResult<ContributionDto>>(paginationResult);

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Endpoint to creating a new Contribution.
    /// </summary>
    /// <param name="contributionDto"></param>
    /// <returns></returns>
    [HttpPost]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ContributionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> CreateContribution([FromBody] CreateContributionDto contributionDto)
    {
        try
        {
            var validation = await _validation.ValidateForCreate(contributionDto);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status404NotFound };
            }

            var createdContribution = await _service.Create(contributionDto);
            var createdContributionDto = _service.ToDto(createdContribution);

            var response = new ApiResponse<ContributionDto>(createdContributionDto,"Successfully Created Contribution");

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    [HttpPost]
    [Route("range")]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(List<ContributionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> CreateContributions([FromBody] List<CreateContributionDto> contributionDto)
    {
        try
        {
            var createdContributions = await _service.CreateRange(contributionDto);
            var createdContributionsDtos = _service.ToDtoRange(createdContributions);

            var response = new ApiResponse<List<ContributionDto>>(createdContributionsDtos);
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
}