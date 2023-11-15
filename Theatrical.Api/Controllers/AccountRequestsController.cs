using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Theatrical.Data.Models;
using Theatrical.Dto.AccountRequestDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Email;
using Theatrical.Services.Security.AuthorizationFilters;
using Theatrical.Services.Validation;
using Files = System.IO.File;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class AccountRequestsController : ControllerBase
{
    private readonly IAccountRequestService _service;
    private readonly IPersonValidationService _personValidation;
    private readonly IAccountRequestValidationService _accountRequestValidation;
    private readonly IUserValidationService _userValidation;
    private readonly IEmailService _emailService;

    public AccountRequestsController(IAccountRequestService service, IPersonValidationService personValidationService,
        IAccountRequestValidationService accountRequestValidationService, IUserValidationService userValidationService,
        IEmailService emailService)
    {
        _service = service;
        _personValidation = personValidationService;
        _accountRequestValidation = accountRequestValidationService;
        _userValidation = userValidationService;
        _emailService = emailService;
    }

    [HttpGet("ClaimsManagers")]
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
    
    /// <summary>
    /// Send the Id of the person you want to claim, and your pdf in Base64 format.
    /// </summary>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("RequestAccount")]
    [TypeFilter(typeof(UserAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> RequestAccount([FromBody] CreateAccountRequestDto requestDto)
    {
        try
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _userValidation.ValidateWithEmail(email!);

            //Edge case that checks if the connected user is deleted/not existent during the request.
            if (user is null)
            {
                var userError = new ApiResponse(ErrorCode.NotFound, "There was an error finding your user account");
                return new ObjectResult(userError) { StatusCode = 404 };
            }
            
            //Checks if the user already have a claimed person.
            var uniquenessValidation = await _userValidation.ValidateUserPersonUniqueness(user.Id);
            if (!uniquenessValidation.Success)
            {
                var uniqueErrorResponse = new ApiResponse(ErrorCode.AlreadyExists, uniquenessValidation.Message!);
                return new ObjectResult(uniqueErrorResponse) { StatusCode = (int)HttpStatusCode.Conflict };
            }

            if (user.Enabled == false)
            {
                var userError = new ApiResponse(ErrorCode.Forbidden, "You can't request an account without verifying your email first.");
                return new ObjectResult(userError) { StatusCode = 403 };
            }
            
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
            
            //Normal flow of code
            var accountRequest = await _service.CreateRequest(person, user, requestDto);

            await _emailService.SendRequestConfirmationEmail(user.Email, person.Fullname);

            ApiResponse response = new ApiResponse<ResponseAccountRequestDto>(accountRequest, "You have made an account request.");

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    [HttpGet]
    [Route("Approve/{requestId}")]
    [TypeFilter(typeof(ClaimsManagerAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> ApproveRequest([FromRoute] int requestId)
    {
        try
        {
            
            var (valReport, accountRequest) = await _accountRequestValidation.FetchAndValidate(requestId);
            if (!valReport.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)valReport.ErrorCode!, valReport.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            if (accountRequest!.ConfirmationStatus != ConfirmationStatus.Active)
            {
                var errorResponse = new ApiResponse(ErrorCode.Forbidden, "You can't take action against a completed request");
                return new ObjectResult(errorResponse){StatusCode = (int) HttpStatusCode.Forbidden};
            }

            //Checks if the person still exists. Also returns the related person.
            var (personReport, person) = await _personValidation.ValidateAndFetch(accountRequest!.PersonId);
            if (!personReport.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)valReport.ErrorCode!, valReport.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            if (person!.IsClaimed)
            {
                var errorResponse = new ApiResponse(ErrorCode.Forbidden, "This person is already claimed");
                return new ObjectResult(errorResponse){StatusCode = (int) HttpStatusCode.Forbidden};
            }

            if (person.ClaimingStatus != ClaimingStatus.InProgress)
            {
                var errorResponse = new ApiResponse(ErrorCode.Forbidden, "This person claiming status is not in progress");
                return new ObjectResult(errorResponse){StatusCode = (int) HttpStatusCode.Forbidden};
            }
            
            //Fetches the email from provided jwt and finds the manager user.
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var (managerReport, managerUser) = await _userValidation.ValidateUser(email);
            
            if (!managerReport.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)managerReport.ErrorCode!, managerReport.Message!);
                return new ObjectResult(errorResponse) { StatusCode = (int)HttpStatusCode.NotFound };
            }

            var (claimantReport, claimantUser) = await _userValidation.ValidateUserById(accountRequest.UserId);
            if (!claimantReport.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)claimantReport.ErrorCode!, claimantReport.Message!);
                return new ObjectResult(errorResponse) { StatusCode = (int)HttpStatusCode.NotFound };
            }
            

            var requestActionDto = new RequestActionDto
            {
                ManagerUser = managerUser!, //Confirmed not null.
                Claimant = claimantUser!,   //Confirmed not null;
                Person = person!,           //Confirmed not null.
                AccountRequest = accountRequest,
                RequestManagerAction = RequestManagerAction.Approve
            };

            
            await _service.RequestAction(requestActionDto);

            await _emailService.SendApprovalEmail(claimantUser!.Email, person.Fullname);

            var apiResponse = new ApiResponse("Successfully assigned the account to the user");

            return new OkObjectResult(apiResponse);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
    
    [HttpGet]
    [Route("Reject/{requestId}")]
    [TypeFilter(typeof(ClaimsManagerAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> RejectRequest([FromRoute] int requestId)
    {
        try
        {
            //code does not pass this point if the role of the user who approves/reject a request is not claims manager.
            
            var (valReport, accountRequest) = await _accountRequestValidation.FetchAndValidate(requestId);
            if (!valReport.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)valReport.ErrorCode!, valReport.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            if (accountRequest!.ConfirmationStatus != ConfirmationStatus.Active)
            {
                var errorResponse = new ApiResponse(ErrorCode.Forbidden, "You can't take action against a completed request");
                return new ObjectResult(errorResponse){StatusCode = (int) HttpStatusCode.Forbidden};
            }

            //Checks if the person still exists. Also returns the related person.
            var (personReport, person) = await _personValidation.ValidateAndFetch(accountRequest!.PersonId);
            if (!personReport.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)valReport.ErrorCode!, valReport.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            if (person!.IsClaimed)
            {
                var errorResponse = new ApiResponse(ErrorCode.Forbidden, "This person is already claimed");
                return new ObjectResult(errorResponse){StatusCode = (int) HttpStatusCode.Forbidden};
            }

            if (person.ClaimingStatus != ClaimingStatus.InProgress)
            {
                //Edge case that should never hit under the normal flow.
                var errorResponse = new ApiResponse(ErrorCode.Forbidden, "This person claiming status is not in progress");
                return new ObjectResult(errorResponse){StatusCode = (int) HttpStatusCode.Forbidden};
            }
            
            //Gets the email from provided jwt and finds the manager user.
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var (managerReport, managerUser) = await _userValidation.ValidateUser(email);
            
            if (!managerReport.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)managerReport.ErrorCode!, managerReport.Message!);
                return new ObjectResult(errorResponse) { StatusCode = (int)HttpStatusCode.NotFound };
            }

            var (claimantReport, claimantUser) = await _userValidation.ValidateUserById(accountRequest.UserId);
            if (!claimantReport.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)claimantReport.ErrorCode!, claimantReport.Message!);
                return new ObjectResult(errorResponse) { StatusCode = (int)HttpStatusCode.NotFound };
            }
            

            var requestActionDto = new RequestActionDto
            {
                ManagerUser = managerUser!, //Confirmed not null.
                Claimant = claimantUser!,   //Confirmed not null;
                Person = person,           
                AccountRequest = accountRequest,
                RequestManagerAction = RequestManagerAction.Reject
            };

            
            await _service.RequestAction(requestActionDto);
            
            await _emailService.SendDisApprovalEmail(claimantUser!.Email, person.Fullname);

            var apiResponse = new ApiResponse("Successfully rejected the account request");

            return new OkObjectResult(apiResponse);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
}