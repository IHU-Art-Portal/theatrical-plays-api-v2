using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
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

            if (verification.ErrorCode == ErrorCode.AlreadyVerified)
            {
                var responseVerified = new ApiResponse(verification.Message!);
                return new OkObjectResult(responseVerified);
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
            var (validationReport, user) = await _validation.ValidateForLogin(loginUserDto);

            if (!validationReport.Success)
            {
                if (validationReport.ErrorCode.Equals(ErrorCode._2FaEnabled))
                {
                    var errorResponse2Fa = new ApiResponse((ErrorCode)validationReport.ErrorCode, validationReport.Message!);

                    //Creates the 2fa code
                    var totpCode = _service.GenerateOTP(user!);
                    
                    //Sends an email to the user with the 2fa code
                    await _emailService.Send2FaVerificationCode(user!, totpCode);
                    
                    //Saves the code.
                    await _service.Save2FaCode(user!, totpCode);
                        
                    return new ObjectResult(errorResponse2Fa){StatusCode = (int) HttpStatusCode.Conflict};
                }
                
                var errorResponse = new ApiResponse((ErrorCode)validationReport.ErrorCode!, validationReport.Message!);
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
    
    
    [HttpPost("enable2fa")]
    public async Task<ActionResult<ApiResponse>> EnableTwoFactorAuth(LoginUserDto userDto)
    {
        var (validation, user) = await _validation.ValidateForLogin(userDto);

        if (!validation.Success)
        {
            if (validation.ErrorCode.Equals(ErrorCode._2FaEnabled))
            {
                var response2FaActivated = new ApiResponse((ErrorCode)validation.ErrorCode!, validation.Message!);
                return new ObjectResult(response2FaActivated) { StatusCode = (int)HttpStatusCode.Conflict };
            }
            
            var errorResponse = new ApiResponse((ErrorCode)validation.ErrorCode!, validation.Message!);
            return new ObjectResult(errorResponse) { StatusCode = (int)HttpStatusCode.NotFound };
        }

        await _service.ActivateTwoFactorAuthentication(user!);

        await _emailService.SendConfirmationEmailTwoFactorActivated(user!.Email);

        var response = new ApiResponse("Two Factor Authentication Activated!");

        return new OkObjectResult(response);
    }

    [HttpOptions("disable2fa")]
    public async Task<ActionResult<ApiResponse>> DisableTwoFactorAuth(LoginUserDto userDto)
    {
        return Ok();
    }

    /// <summary>
    /// Use this after getting your f2a code from email.
    /// Verifies the code,
    /// Generates a login token (jwt),
    /// Sends appropriate reply.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpPost("login/2fa/{code}")]
    public async Task<ActionResult<ApiResponse>> Login2Fa([FromRoute]int code)
    {
        var (validation, user) = await _validation.VerifyOtp(code.ToString());

        if (!validation.Success)
        {
            var errorResponse = new ApiResponse((ErrorCode)validation.ErrorCode!, validation.Message!);
            return new ObjectResult(errorResponse){StatusCode = (int)HttpStatusCode.Unauthorized};
        }

        var jwtDto = _service.GenerateToken(user!);
        
        var response = new ApiResponse<JwtDto>(jwtDto, validation.Message!);
        
        return new OkObjectResult(response);
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