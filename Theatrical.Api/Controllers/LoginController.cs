using System.Net;
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
        return StatusCode((int)HttpStatusCode.NotImplemented, "This function is not implemented yet and might be subject to changes.");
    }
}