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
        await _service.Create(role);

        var response = new TheatricalResponse("Successfully Created Role");
        
        return new OkObjectResult(response);
    }

    [HttpGet]
    public async Task<ActionResult<TheatricalResponse>> GetRoles()
    {
        var roles = await _service.Get();

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
}