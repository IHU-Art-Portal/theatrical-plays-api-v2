using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.PersonDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.PerformersService;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
[EnableCors("AllowOrigin")]
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
        try
        {
            var (validation, person) = await _validation.ValidateAndFetch(id);

            if (!validation.Success)
            {
                ApiResponse errorResponse = new ApiResponse((ErrorCode)validation.ErrorCode!, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var performerDto = _service.ToDto(person!);

            ApiResponse response = new ApiResponse<PersonDto>(performerDto);

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Retrieves all performers if pagination parameters are not specified
    /// </summary>
    /// <param name="page">Optional. The page number for pagination</param>
    /// <param name="size">Optional. THe page size for pagination</param>
    /// <returns>TheatricalResponse&lt;PerformersPaginationDto&gt; object containing paginated items.</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetPeople(int? page, int? size)
    {
        try
        {
            var peopleDto = await _service.GetAndPaginate(page, size);

            ApiResponse response = new ApiResponse<PaginationResult<PersonDto>>(peopleDto);

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Endpoint to creating a new Person.
    /// System 2 for Python, 3 for C#, 10 for Spring, 11 for Android, 12 for iOS, 13 for Web App.
    /// </summary>
    /// <param name="createPersonDto">Fullname, ImageLinks (if any), System ID</param>
    /// <returns></returns>
    [HttpPost]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> CreatePerson([FromBody] CreatePersonDto createPersonDto)
    {
        try
        {
            var (validation, correctedCreatePersonDto) = await _validation.ValidateForCreate(createPersonDto);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)validation.ErrorCode!, validation.Message!);
                
                return new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status400BadRequest};
            }

            var createdPerson = await _service.Create(correctedCreatePersonDto!);
            var createdPersonDto = _service.ToDto(createdPerson);

            var response = new ApiResponse<PersonDto>(createdPersonDto,"Successfully Created Person");

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Search function not implemented yet.
    /// </summary>
    /// <param name="role"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("search")]
    public ActionResult GetPerformersRole(string? role, int? page, int? size)
    {
        return StatusCode((int)HttpStatusCode.NotImplemented, "This function is not implemented yet and might be subject to changes.");
    }

    /// <summary>
    /// Endpoint to fetching all Person(s) by a specific role.
    /// Pagination Available.
    /// </summary>
    /// <param name="role"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("role/{role}")]
    public async Task<ActionResult<ApiResponse>> GetPeopleByRole(string role, int? page, int? size)
    {
        try
        {
            var (validation, people) = await _validation.ValidateForFetchRole(role);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)validation.ErrorCode!, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var paginationResult = _service.PaginateAndProduceDtos(people!, page, size);

            var response = new ApiResponse<PaginationResult<PersonDto>>(paginationResult);
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Endpoint to fetching a Person by their initials.
    /// </summary>
    /// <param name="letters"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("initials/{letters}")]
    public async Task<ActionResult<ApiResponse>> GetPeopleByInitialLetter(string letters, int? page, int? size)
    {
        try
        {
            var (validation, persons) = await _validation.ValidateForInitials(letters);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new ObjectResult(errorResponse){StatusCode = 404};
            }

            var paginatedResult = _service.PaginateAndProduceDtos(persons!, page, size);

            var response = new ApiResponse<PaginationResult<PersonDto>>(paginatedResult);

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Endpoint to fetching all Productions a Person participates in.
    /// Pagination Available.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id}/productions")]
    public async Task<ActionResult<ApiResponse>> GetPersonProductions(int id, int? page, int? size)
    {
        try
        {
            var (validation, productions) = await _validation.ValidatePersonsProductions(id);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var paginationResult = _service.PaginateContributionsOfPerson(productions!, page, size);
            
            var response = new ApiResponse<PaginationResult<PersonProductionsRoleInfo>>(paginationResult);
            
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Endpoint to fetching all photos by Person's Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id}/photos")]
    public async Task<ActionResult<ApiResponse>> GetPersonsPhotos(int id)
    {
        try
        {
            var (validation, images) = await _validation.ValidatePersonsPhotos(id);

            if (!validation.Success)
            {
                if (validation.ErrorCode.Equals(ErrorCode.NoAvailablePhotos))
                {
                    var emptyArray = new List<Person>();
                    //returns 200 if the person exists but has no photos.
                    var errorNotFoundPhotos = new ApiResponse<List<Person>>(emptyArray, validation.Message!);
                    return new OkObjectResult(errorNotFoundPhotos);
                }
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var imagesDto = _service.ImagesToDto(images!);
            
            var response = new ApiResponse<List<ImageDto>>(imagesDto);
            
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
    
    /// <summary>
    /// Endpoint to deleting a Person by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id}")]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> DeletePerson(int id)
    {
        try
        {
            var (validation, performer) = await _validation.ValidateForDelete(id);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)validation.ErrorCode!, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            await _service.Delete(performer!);
            ApiResponse response = new ApiResponse(message: $"Person with ID: {id} has been deleted!");

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            if (e.HResult.Equals(-2146233088))
            {
                return new ObjectResult(new ApiResponse(ErrorCode.ServerError, "Object has already been deleted, but due to temporary caching you get this error. You may disregard this error message.")) { StatusCode = 500 };
            }

            return new ObjectResult(new ApiResponse(ErrorCode.ServerError, e.Message));
        }
    }

    [HttpGet]
    [Route("photos")]
    public async Task<ActionResult<ApiResponse>> GetPhotos()
    {
        try
        {
            var images = await _service.GetImages();

            var apiResponse = new ApiResponse<List<Image>>(images);
            
            return new OkObjectResult(apiResponse);
        }
        catch (Exception e)
        {
            return new ObjectResult(new ApiResponse(ErrorCode.ServerError, e.Message));
        }
    }
}