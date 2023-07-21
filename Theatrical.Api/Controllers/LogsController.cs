using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.LogDtos;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ILogService _service;

    public LogsController(ILogService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetLogs(int? page, int? size)
    {
        try
        {
            var logs = await _service.GetLogs();

            var paginatedLogs = _service.Paginate(page, size, logs);
            
            var response = new ApiResponse<PaginationResult<LogDto>>(paginatedLogs);
            
            return response;
        }
        catch (Exception e)
        {
            var exceptionResponse = new ApiResponse(ErrorCode.ServerError, e.Message);
            return new ObjectResult(exceptionResponse){StatusCode = 500};
        }
    }
}