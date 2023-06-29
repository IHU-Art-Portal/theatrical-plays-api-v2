using System.Net;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.PerformersService;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly IPersonService _service;
    private readonly IPersonValidationService _validation;

    public PeopleController(IPersonService service, IPersonValidationService validation)
    {
        _service = service;
        _validation = validation;
    }

    /// <summary>
    /// Retrieves performer information by their Id
    /// </summary>
    /// <param name="id">int</param>
    /// <returns>TheatricalResponse&lt;PerformerDto&gt; object containing performer data.</returns>
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ApiResponse<PersonDto>>> GetPerson(int id)
    {
        var (validation, person) = await _validation.ValidateAndFetch(id);

        if (!validation.Success)
        {
            ApiResponse errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }
        
        var performerDto = _service.ToDto(person);

        ApiResponse response = new ApiResponse<PersonDto>(performerDto);
        
        return new ObjectResult(response);
    }

    /// <summary>
    /// Retrieves all performers if pagination parameters are not specified
    /// </summary>
    /// <param name="page">Optional. The page number for pagination</param>
    /// <param name="size">Optional. THe page size for pagination</param>
    /// <returns>TheatricalResponse&lt;PerformersPaginationDto&gt; object containing paginated items.</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PerformersPaginationDto>>> GetPersons(int? page, int? size)
    {

        PerformersPaginationDto performersDto = await _service.Get(page, size);
        
        ApiResponse response = new ApiResponse<PerformersPaginationDto>(performersDto);
        
        return new ObjectResult(response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreatePerson([FromBody] CreatePerformerDto createPerformerDto)
    {
        await _service.Create(createPerformerDto);

        var response = new ApiResponse("Successfully Created Person");
        
        return new OkObjectResult(response);
    }

    [HttpGet]
    [Route("search")]
    public ActionResult GetPerformersRole(string role, int? page, int? size)
    {
        return StatusCode((int)HttpStatusCode.NotImplemented, "This function is not implemented yet and might be subject to changes.");
    }

    [HttpGet]
    [Route("{role}")]
    public async Task<ActionResult> GetPeopleByRole(string role)
    {
        var (validation, people) = await _validation.ValidateForFetchRole(role);

        if (!validation.Success)
        {
            var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }

        var response = new ApiResponse<List<Person>>(people);
        return new OkObjectResult(response);
    }

    /*[HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse>> DeletePerformer(int id)
    {
        var (validation, performer) = await _validation.ValidateForDelete(id);

        if (!validation.Success)
        {
            var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }
        
        await _service.Delete(performer);
        ApiResponse response = new ApiResponse(message: $"Person with ID: {id} has been deleted!");
        
        return new OkObjectResult(response);
    }*/
}