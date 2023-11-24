using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IRoleValidationService
{
    Task<(ValidationReport report, Role? role)> ValidateForDelete(int id);
    Task<(ValidationReport report, Role? role)> ValidateForDelete(string roleToDelete);
    Task<(ValidationReport report, List<Role>? roles)> ValidateForFetch();
    Task<ValidationReport> ValidateForCreate(string searchRole);
}

public class RoleValidationService : IRoleValidationService
{
    private readonly IRoleRepository _repository;

    public RoleValidationService(IRoleRepository repository)
    {
        _repository = repository;
    }
    public async Task<(ValidationReport report, Role? role)> ValidateForDelete(int id)
    {
        var report = new ValidationReport();
        var role = await _repository.GetRole(id);

        if (role is null)
        {
            report.Success = false;
            report.Message = "Role not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Role found and marked for deletion";
        return (report, role);
    }

    public async Task<(ValidationReport report, Role? role)> ValidateForDelete(string roleToDelete)
    {
        var report = new ValidationReport();
        var role = await _repository.GetRoleByName(roleToDelete);

        if (role is null)
        {
            report.Success = false;
            report.Message = "Role not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Role found and marked for deletion";
        return (report, role);
    }
    
    public async Task<(ValidationReport report, List<Role>? roles)> ValidateForFetch()
    {
        var report = new ValidationReport();
        var roles = await _repository.GetRoles();

        if (!roles.Any())
        {
            report.Success = false;
            report.Message = "There are no roles available";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Roles found";
        return (report, roles);
    }
    
    public async Task<ValidationReport> ValidateForCreate(string searchRole)
    {
        var report = new ValidationReport();
        var role = await _repository.GetRoleByName(searchRole);
        
        if (role is not null)
        {
            report.Success = false;
            report.Message = $"Role: {searchRole} already exists";
            return report;
        }

        report.Success = true;
        report.Message = "Role can be created";
        return report;
    }
}