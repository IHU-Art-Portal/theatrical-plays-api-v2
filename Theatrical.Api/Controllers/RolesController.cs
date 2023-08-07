using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Dto.RoleDtos;
using Theatrical.Services;
using Theatrical.Services.Validation;

namespace Theatrical.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowOrigin")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _service;
    private readonly IRoleValidationService _validation;

    public RolesController(IRoleService service, IRoleValidationService validation)
    {
        _service = service;
        _validation = validation;
    }
    
    /// <summary>
    /// Endpoint to creating a new role.
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("{role}")]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult<ApiResponse>> CreateRole(string role)
    {
        try
        {
            var rolelowercase = role.ToLower();
            var validation = await _validation.ValidateForCreate(rolelowercase);

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.AlreadyExists, validation.Message!);
                return new ConflictObjectResult(errorResponse);
            }

            await _service.Create(rolelowercase);

            var response = new ApiResponse($"Successfully Created Role: {rolelowercase}");

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /// <summary>
    /// Endpoint to fetching all roles.
    /// Pagination available.
    /// Oldest to newest
    /// </summary>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetRoles(int? page,  int? size)
    {
        try
        {
            var (validation, roles) = await _validation.ValidateForFetch();

            if (!validation.Success)
            {
                var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message!);
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            }

            var rolesDto = _service.ToDto(roles!);

            var paginationResult = _service.Paginate(page, size, rolesDto);
            
            var response = new ApiResponse<PaginationResult<RoleDto>>(paginationResult);

            return new ObjectResult(response);
        }
        catch (Exception e)
        {
            var unexpectedResponse = new ApiResponse(ErrorCode.ServerError, e.Message);

            return new ObjectResult(unexpectedResponse){StatusCode = StatusCodes.Status500InternalServerError};
        }
    }

    /*[HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteRole(int id)
    {
        var (validation, role) = await _validation.ValidateForDelete(id);

        if (!validation.Success)
        {
            var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }

        await _service.Delete(role);
        ApiResponse response = new ApiResponse(message: $"Role with ID: {id} has been deleted!");

        return new OkObjectResult(response);
    }
    
    [HttpDelete]
    [TypeFilter(typeof(CustomAuthorizationFilter))]
    [Route("@name/{roleToDelete}")]
    public async Task<ActionResult<ApiResponse>> DeleteRoleByName(string roleToDelete)
    {
        var lowercaseRole = roleToDelete.ToLower();
        var (validation, role) = await _validation.ValidateForDelete(lowercaseRole);

        if (!validation.Success)
        {
            var errorResponse = new ApiResponse(ErrorCode.NotFound, validation.Message);
            return new ObjectResult(errorResponse){StatusCode = 404};
        }

        await _service.Delete(role);
        ApiResponse response = new ApiResponse(message: $"Role: {lowercaseRole} has been deleted!");

        return new OkObjectResult(response);
    }*/
}