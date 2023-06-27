using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IPersonValidationService
{
    Task<(ValidationReport report, Person? performer)> ValidateAndFetch(int performerId);
    Task<(ValidationReport report, Person? performer)> ValidateForDelete(int performerId);
}

public class PersonValidationService : IPersonValidationService
{
    private readonly IPersonRepository _repository;

    public PersonValidationService(IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<(ValidationReport report, Person? performer)> ValidateAndFetch(int performerId)
    {
        var performer = await _repository.Get(performerId);
        var report = new ValidationReport();

        if (performer is null)
        {
            report.Success = false;
            report.Message = "Person not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Person exists";
        return (report, performer);
    }

    public async Task<(ValidationReport report, Person? performer)> ValidateForDelete(int performerId)
    {
        var performer = await _repository.Get(performerId);
        var report = new ValidationReport();
        
        if (performer is null)
        {
            report.Success = false;
            report.Message = "Person not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Person exists and marked for deletion";
        return (report, performer);
    }
}