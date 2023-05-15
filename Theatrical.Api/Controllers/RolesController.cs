using Microsoft.AspNetCore.Mvc;
using Theatrical.Services;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _service;

    public RolesController(IRoleService service)
    {
        _service = service;
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
        await _service.Delete(id);
        return NoContent();
    }
}