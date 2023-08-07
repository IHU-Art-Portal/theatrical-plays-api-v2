using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.LoginDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Email;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class UserController : ControllerBase
{
    private readonly IUserValidationService _validation;
    private readonly IUserService _service;
    private readonly IEmailService _emailService;

    public UserController(IUserValidationService validation, IUserService service, IEmailService emailService)
    {
        _validation = validation;
        _service = service;
        _emailService = emailService;
    }
    
    /// <summary>
    /// Use this method to register.
    /// Use 1 for admin account or
    /// Use 2 for user account or
    /// Use 3 for developer account.
    /// If you don't define role, user account will be created.
    /// </summary>
    /// <param name="registerUserDto"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterUserDto registerUserDto)
    {
        try
        {
            var validation = await _validation.ValidateForRegister(registerUserDto);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)validation.ErrorCode!, validation.Message!);
                return new BadRequestObjectResult(errorResponse);
            }

            //Generate the verification token
            var verificationToken = Guid.NewGuid().ToString();

            //Send confirmation email to the registered user.
            await _emailService.SendConfirmationEmailAsync(registerUserDto.Email, verificationToken);
            
            
            var userCreated = await _service.Register(registerUserDto, verificationToken);
            var response = new ApiResponse<UserDtoRole>(userCreated, "Successfully Registered!");
            
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = (int)HttpStatusCode.InternalServerError}; 
        }
    }

    /// <summary>
    /// Verification link points to this endpoint.
    /// Verifies the user and enabled the account.
    /// </summary>
    /// <param name="token">verification code</param>
    /// <returns></returns>
    [HttpGet("verify")]
    public async Task<ActionResult<ApiResponse>> VerifyEmail([FromQuery]string token)
    {
        try
        {
            
            var (verification, user) = await _validation.VerifyEmailToken(token);
            
            if (!verification.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)verification.ErrorCode!, verification.Message!);
                return new ObjectResult(errorResponse){StatusCode = (int)HttpStatusCode.BadRequest};
            }

            await _service.EnableAccount(user!);
            
            var response = new ApiResponse(verification.Message!);
            
            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = (int)HttpStatusCode.InternalServerError}; 
        }
    }

    /// <summary>
    /// Use this to login in.
    /// Logging in provides a JWT to user.
    /// Use the token for locked actions.
    /// </summary>
    /// <param name="loginUserDto"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse>> Login([FromBody]LoginUserDto loginUserDto)
    {
        try
        {
            var (report, user) = await _validation.ValidateForLogin(loginUserDto);

            if (!report.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)report.ErrorCode!, report.Message!);
                return new ObjectResult(errorResponse) { StatusCode = (int)HttpStatusCode.NotFound };
            }

            var jwtDto = _service.GenerateToken(user!);

            var response = new ApiResponse<JwtDto>(jwtDto);

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = (int)HttpStatusCode.InternalServerError};
        }
    }

    /// <summary>
    /// Provides the balance of a user.
    /// </summary>
    /// <param name="id">user's id</param>
    /// <returns>Available User Credits</returns>
    [HttpGet("{id}/balance")]
    public async Task<ActionResult<ApiResponse>> Balance([FromRoute]int id)
    {
        try
        {
            var (validationReport, credits) = await _validation.ValidateBalance(id);

            if (!validationReport.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)validationReport.ErrorCode!, validationReport.Message!);
                return new ObjectResult(errorResponse) { StatusCode = (int)HttpStatusCode.NotFound };
            }

            var response = new ApiResponse<string>($"You have {credits} credits.");

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = (int)HttpStatusCode.InternalServerError};
        }
    }

}