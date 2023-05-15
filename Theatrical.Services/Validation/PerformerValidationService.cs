using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IPerformerValidationService
{
    Task<(ValidationReport report, Performer? performer)> ValidateAndFetch(int performerId);
    Task<(ValidationReport report, Performer? performer)> ValidateForDelete(int performerId);
}

public class PerformerValidationService : IPerformerValidationService
{
    private readonly IPerformerRepository _repository;

    public PerformerValidationService(IPerformerRepository repository)
    {
        _repository = repository;
    }

    public async Task<(ValidationReport report, Performer? performer)> ValidateAndFetch(int performerId)
    {
        var performer = await _repository.Get(performerId);
        var report = new ValidationReport();

        if (performer is null)
        {
            report.Success = false;
            report.Message = "Performer not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Performer exists";
        return (report, performer);
    }

    public async Task<(ValidationReport report, Performer? performer)> ValidateForDelete(int performerId)
    {
        var performer = await _repository.Get(performerId);
        var report = new ValidationReport();
        
        if (performer is null)
        {
            report.Success = false;
            report.Message = "Performer not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Performer exists and marked for deletion";
        return (report, performer);
    }
}