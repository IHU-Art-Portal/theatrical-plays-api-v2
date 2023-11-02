using System.Security.Claims;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.AccountRequestDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Security.AuthorizationFilters;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class AccountRequestsController : ControllerBase
{
    private readonly IAccountRequestService _service;
    private readonly IPersonValidationService _personValidation;

    public AccountRequestsController(IAccountRequestService service, IPersonValidationService personValidationService)
    {
        _service = service;
        _personValidation = personValidationService;
    }

    [HttpGet]
    [TypeFilter(typeof(ClaimsManagerAndAdminAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> ShowAll([FromQuery] ConfirmationStatus? status)
    {
        try
        {
            var accountRequests = await _service.GetAll(status);

            var apiResponse = new ApiResponse<List<AccountRequestDto>>(accountRequests);

            return new OkObjectResult(apiResponse);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
    
    [HttpPost]
    [Route("RequestAccount")]
    [TypeFilter(typeof(UserAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> RequestAccount([FromBody] CreateAccountRequestDto requestDto)
    {
        try
        {
            var (validation, person) = await _personValidation.ValidateAndFetch(requestDto.PersonId);

            if (!validation.Success)
            {
                ApiResponse errorResponse = new ApiResponse((ErrorCode)validation.ErrorCode!, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            if (person!.ClaimingStatus != ClaimingStatus.Available)
            {
                var claimingError = new ApiResponse(ErrorCode.Forbidden, "The account you tried to request is unavailable.");
                return new ObjectResult(claimingError) { StatusCode = 403 };
            }
            
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _personValidation.ValidateWithEmail(email!);

            //Edge case that checks if the connected user is deleted/not existent during the request.
            if (user is null)
            {
                var userError = new ApiResponse(ErrorCode.NotFound, "There was an error finding your user account");
                return new ObjectResult(userError) { StatusCode = 404 };
            }

            //Normal flow of code
            var accountRequest = await _service.CreateRequest(person, user, requestDto);

            ApiResponse response = new ApiResponse<ResponseAccountRequestDto>(accountRequest, "You have made an account request.");

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
}