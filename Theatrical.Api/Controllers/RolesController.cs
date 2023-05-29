using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.LoginDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _service;
    private readonly IRoleValidationService _validation;

    public RolesController(IRoleService service, IRoleValidationService validation)
    {
        _service = service;
        _validation = validation;
    }
    
    [HttpPost]
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    [Route("{role}")]
    public async Task<ActionResult<TheatricalResponse>> CreateRole(string role)
    {
        var rolelowercase = role.ToLower();
        var validation = await _validation.ValidateForCreate(rolelowercase);

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.AlreadyExists, validation.Message!);
            return new ConflictObjectResult(errorResponse);
        }
        
        await _service.Create(rolelowercase);

        var response = new TheatricalResponse($"Successfully Created Role: {rolelowercase}");
        
        return new OkObjectResult(response);
    }

    [HttpGet]
    public async Task<ActionResult<TheatricalResponse>> GetRoles()
    {
        var (validation, roles) = await _validation.ValidateForFetch();

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }

        var response = new TheatricalResponse<List<Role>>(roles);
        
        return new ObjectResult(response);
    }

    [HttpDelete]
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    [Route("{id}")]
    public async Task<ActionResult<TheatricalResponse>> DeleteRole(int id)
    {
        var (validation, role) = await _validation.ValidateForDelete(id);

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }

        await _service.Delete(role);
        TheatricalResponse response = new TheatricalResponse(message: $"Role with ID: {id} has been deleted!");

        return new OkObjectResult(response);
    }
    
    [HttpDelete]
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    [Route("@name/{roleToDelete}")]
    public async Task<ActionResult<TheatricalResponse>> DeleteRoleByName(string roleToDelete)
    {
        var lowercaseRole = roleToDelete.ToLower();
        var (validation, role) = await _validation.ValidateForDelete(lowercaseRole);

        if (!validation.Success)
        {
            var errorResponse = new TheatricalResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }

        await _service.Delete(role);
        TheatricalResponse response = new TheatricalResponse(message: $"Role: {lowercaseRole} has been deleted!");

        return new OkObjectResult(response);
    }
}