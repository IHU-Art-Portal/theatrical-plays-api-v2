using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.OrganizerDtos;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Security.AuthorizationFilters;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class OrganizersController : ControllerBase
{
    private readonly IOrganizerService _service;
    private readonly IOrganizerValidationService _validation;
    
    public OrganizersController(IOrganizerService service, IOrganizerValidationService validation)
    {
        _service = service;
        _validation = validation;
    }
    
    /// <summary>
    /// Endpoint to creating a new Organizer.
    /// </summary>
    /// <param name="organizerCreateDto"></param>
    /// <returns></returns>
    [HttpPost]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> CreateOrganizer([FromBody] OrganizerCreateDto organizerCreateDto)
    {
        try
        {
            await _service.Create(organizerCreateDto);

            var response = new ApiResponse("Successfully created Organizer");

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
    [ProducesResponseType(typeof(OrganizersCreationResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> CreateOrganizers([FromBody] List<OrganizerCreateDto> organizerCreateDtos)
    {
        try
        {
            var existingOrganizers = await _service.GetOrganizersByNames(organizerCreateDtos.Select(dto => dto.Name).ToList());

            var (updatedOrganizers,  createdOrganizers, responseList) = await _service.CreateListsAndUpdateCreate(existingOrganizers, organizerCreateDtos);

            var organizersCreationResponseDto = new OrganizersCreationResponseDto
            {
                CreatedCount = createdOrganizers.Count,
                UpdatedCount = updatedOrganizers.Count,
                OrganizerDtos = responseList
            };

            return Ok(new ApiResponse<OrganizersCreationResponseDto>(organizersCreationResponseDto));
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Endpoint to fetching all Organizer(s).
    /// Pagination Available.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(PaginationResult<OrganizerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetOrganizers(int? page, int? size)
    {
        try
        {
            var (report, organizers) = await _validation.ValidateAndFetch();

            if (!report.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, report.Message!);
                return new NotFoundObjectResult(errorResponse);
            }

            var organizerDtos = _service.ToDto(organizers!);

            var paginationResult = _service.Paginate(page, size, organizerDtos);

            var response = new ApiResponse<PaginationResult<OrganizerDto>>(paginationResult);

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
    
    /*[HttpDelete]
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteOrganizer(int id)
    {
        
        var (report, organizer) = await _validation.ValidateForDelete(id);

        if (!report.Success)
        {
            var errorResponse = new ApiResponse(ErrorCode.NotFound, report.Message);
            return new NotFoundObjectResult(errorResponse);
        }

        await _service.Delete(organizer);
        var response = new ApiResponse<Organizer>(organizer, message: "Organizer has been deleted");
        return new OkObjectResult(response);
    }*/
}