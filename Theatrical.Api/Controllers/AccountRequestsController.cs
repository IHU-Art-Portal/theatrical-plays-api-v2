using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Data.Models;
using Theatrical.Dto.AccountRequestDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class AccountRequestsController : ControllerBase
{
    private readonly IAccountRequestService _service;

    public AccountRequestsController(IAccountRequestService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> ShowAll([FromQuery] ConfirmationStatus? status)
    {
        var accountRequests = await _service.GetAll(status);

        var apiResponse = new ApiResponse<List<AccountRequestDto>>(accountRequests);

        return new OkObjectResult(apiResponse);
    }
}