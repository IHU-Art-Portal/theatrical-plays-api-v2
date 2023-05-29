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
    public async Task<ActionResult<TheatricalResponse>> Register([FromBody] UserDto userDto)
    {
        
        var validation = await _validation.ValidateForRegister(userDto);
        
        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.AlreadyExists, validation.Message);
            return new ConflictObjectResult(errorResponse);
        }

        var userCreated = await _service.Register(userDto);
        var response = new TheatricalResponse<UserDtoRole>(userCreated,"Successfully Registered!");
        
        return new OkObjectResult(response);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<TheatricalResponse>> Login([FromBody]UserDto userDto)
    {
        var (report, user) = await _validation.ValidateForLogin(userDto);

        if (!report.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, report.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }
       
        var jwtDto = _service.GenerateToken(user);
        
        var response = new TheatricalResponse<JwtDto>(jwtDto);
        
        return new OkObjectResult(response);
    }
}