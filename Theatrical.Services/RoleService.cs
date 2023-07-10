using Theatrical.Data.Models;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.RoleDtos;
using Theatrical.Services.Pagination;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IRoleService
{
    Task Create(string role);
    Task Delete(Role roles);
    List<RoleDto> ToDto(List<Role> roles);
    PaginationResult<RoleDto> Paginate(int? page, int? size, List<RoleDto> rolesDto);
}

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IPaginationService _pagination;

    public RoleService(IRoleRepository repository, IPaginationService paginationService)
    {
        _repository = repository;
        _pagination = paginationService;
    }

    public async Task Create(string role)
    {
        var _role = new Role();
        _role.Role1 = role;
        await _repository.CreateRole(_role);
    }

    public async Task Delete(Role role)
    {
        await _repository.DeleteRole(role);
    }

    public List<RoleDto> ToDto(List<Role> roles)
    {
        var rolesDto = roles.Select(role => new RoleDto
        {
            Id = role.Id,
            Role = role.Role1,
            SystemId = role.SystemId,
            Timestamp = role.Timestamp
        }).ToList();

        return rolesDto;
    }

    public PaginationResult<RoleDto> Paginate(int? page, int? size, List<RoleDto> rolesDto)
    {
        var paginationResult = _pagination.GetPaginated(page, size, rolesDto, items =>
        {
            return items.Select(role => new RoleDto
            {
                Id = role.Id,
                Role = role.Role,
                SystemId = role.SystemId,
                Timestamp = role.Timestamp
            });
        });

        return paginationResult;
    }
}