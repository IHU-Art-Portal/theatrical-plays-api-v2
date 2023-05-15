using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IRoleValidationService
{
    Task<(ValidationReport report, Role? role)> ValidateForDelete(int id);
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
}