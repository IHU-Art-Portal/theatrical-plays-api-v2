using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.LoginDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class UserController : ControllerBase
{
    private readonly IUserValidationService _validation;
    private readonly IUserService _service;

    public UserController(IUserValidationService validation, IUserService service)
    {
        _validation = validation;
        _service = service;
    }
    
    /// <summary>
    /// Use this method to register.
    /// Use 1 for admin account or
    /// Use 2 for user account.
    /// If you don't define role, user account will be created.
    /// </summary>
    /// <param name="registerUserDto"></param>
    /// <param name="role">Integer number for role</param>
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

            var userCreated = await _service.Register(registerUserDto);
            var response = new ApiResponse<UserDtoRole>(userCreated, "Successfully Registered!");

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message.ToString());

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse>> Login([FromBody]LoginUserDto loginUserDto)
    {
        try
        {
            var (report, user) = await _validation.ValidateForLogin(loginUserDto);

            if (!report.Success)
            {
                var errorResponse = new ApiResponse((ErrorCode)report.ErrorCode!, report.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var jwtDto = _service.GenerateToken(user!);

            var response = new ApiResponse<JwtDto>(jwtDto);

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message.ToString());

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }
}