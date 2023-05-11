using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.LoginDtos;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{

    [HttpPost]
    public IActionResult Login([FromBody]UserLoginDto userLoginDto)
    {
        return Ok();
    }
}