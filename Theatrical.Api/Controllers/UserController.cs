using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.LoginDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserValidationService _validation;
    private readonly IUserService _service;

    public UserController(IUserValidationService validation, IUserService service)
    {
        _validation = validation;
        _service = service;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse>> Register([FromBody] UserDto userDto)
    {
        try
        {
            var validation = await _validation.ValidateForRegister(userDto);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.AlreadyExists, validation.Message!);
                return new ConflictObjectResult(errorResponse);
            }

            var userCreated = await _service.Register(userDto);
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
    public async Task<ActionResult<ApiResponse>> Login([FromBody]UserDto userDto)
    {
        try
        {
            var (report, user) = await _validation.ValidateForLogin(userDto);

            if (!report.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, report.Message);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var jwtDto = _service.GenerateToken(user);

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