using Theatrical.Data.Models;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.RoleDtos;
using Theatrical.Services.Pagination;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IRoleService
{
    Task<Role> Create(CreateRoleDto createRoleDto);
    Task Delete(Role roles);
    List<RoleDto> ToDtoRange(List<Role> roles);
    PaginationResult<RoleDto> Paginate(int? page, int? size, List<RoleDto> rolesDto);
    Task<(List<Role> rolesAdded, List<Role> existingRoles)> CreateRange(List<CreateRoleDto> createRoleDtos);
    List<RolesDtoShortened> ToDtoRangeShortened(List<Role> roles);
    RolesDtoShortened ToDto(Role roleAdded);
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

    public async Task<Role> Create(CreateRoleDto createRoleDto)
    {
        var roleToAdd = new Role
        {
            Role1 = createRoleDto.Role,
            SystemId = createRoleDto.SystemId
        };
        return await _repository.CreateRole(roleToAdd);
    }

    public async Task Delete(Role role)
    {
        await _repository.DeleteRole(role);
    }

    public List<RoleDto> ToDtoRange(List<Role> roles)
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
    
    public List<RolesDtoShortened> ToDtoRangeShortened(List<Role> roles)
    {
        var rolesDto = roles.Select(role => new RolesDtoShortened
        {
            Id = role.Id,
            Role = role.Role1,
        }).ToList();

        return rolesDto;
    }

    public RolesDtoShortened ToDto(Role roleAdded)
    {
        var roleDto = new RolesDtoShortened
        {
            Id = roleAdded.Id,
            Role = roleAdded.Role1
        };
        return roleDto;
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

    public async Task<(List<Role> rolesAdded, List<Role> existingRoles)> CreateRange(List<CreateRoleDto> createRoleDtos)
    {
        List<Role> existingRoles = await _repository.GetRoles();
        
        var existingRoleNames = existingRoles.Select(role => role.Role1).ToList();
        
        var newRoles = createRoleDtos
            .Where(dto => !existingRoleNames.Contains(dto.Role))
            .ToList();
        
        var rolesToAdd = newRoles.Select(dto => new Role
        {
            Role1 = dto.Role,
            SystemId = dto.SystemId
        }).ToList();

        var rolesAdded = await _repository.CreateRoleRange(rolesToAdd);

        var existingRolesFromDto = createRoleDtos
            .Where(dto => existingRoles.Any(role => role.Role1 == dto.Role))
            .Select(dto => existingRoles.First(role => role.Role1 == dto.Role))
            .ToList();
        
        return (rolesAdded, existingRolesFromDto);
    }
}