﻿using System.Net;
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
    public async Task<ActionResult> Login([FromBody]UserDto userDto)
    {
        return StatusCode((int)HttpStatusCode.NotImplemented, "This function is not implemented yet and might be subject to changes.");
    }
}