using Microsoft.AspNetCore.Mvc;
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
    [Route("{role}")]
    public async Task<ActionResult> CreateRole(string role)
    {
        await _service.Create(role);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult> GetRoles()
    {
        var roles = await _service.Get();
        
        return Ok(roles);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteRole(int id)
    {
        var (validation, role) = await _validation.ValidateForDelete(id);

        if (!validation.Success)
        {
            return NotFound(validation.Message);
        }
        
        await _service.Delete(role);
        return NoContent();
    }
}