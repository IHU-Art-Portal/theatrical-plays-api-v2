using Theatrical.Dto.LoginDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IUserValidationService
{
    Task<ValidationReport> ValidateForRegister(UserDto userdto);
}

public class UserValidationService : IUserValidationService
{
    private readonly IUserRepository _repository;

    public UserValidationService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<ValidationReport> ValidateForRegister(UserDto userdto)
    {
        var report = new ValidationReport();
        var user = await _repository.Get(userdto.Username);

        if (user is not null)
        {
            report.Message = "Username taken!";
            report.Success = false;
            return report;
        }

        report.Message = "User with this username can be created";
        report.Success = true;
        return report;
    }
}