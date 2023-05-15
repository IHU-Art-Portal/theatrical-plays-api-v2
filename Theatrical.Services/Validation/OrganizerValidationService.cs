using Theatrical.Data.Models;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;


public interface IOrganizerValidationService
{
    Task<(ValidationReport report, Organizer? organizer)> ValidateAndFetch(int id);
    Task<(ValidationReport report, List<Organizer>? organizers)> ValidateAndFetch();
    Task<(ValidationReport report, Organizer? organizer)> ValidateForDelete(int organizerId);
}

public class OrganizerValidationService : IOrganizerValidationService
{
    private readonly IOrganizerRepository _repository;

    public OrganizerValidationService(IOrganizerRepository repository)
    {
        _repository = repository;
    }

    public async Task<(ValidationReport report, Organizer? organizer)> ValidateAndFetch(int id)
    {
        var report = new ValidationReport();
        var organizer = await _repository.Get(id);

        if (organizer is null)
        {
            report.Message = "Organizer not found!";
            report.Success = false;
            return (report, null);
        }

        report.Success = true;
        report.Message = "Organizer exists!";
        return (report, organizer);
    }

    public async Task<(ValidationReport report, List<Organizer>? organizers)> ValidateAndFetch()
    {
        var report = new ValidationReport();
        var organizersFromDb = await _repository.Get();

        if (!organizersFromDb.Any())
        {
            report.Success = false;
            report.Message = "Not found any organizers";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Organizers found";
        return (report, organizersFromDb);
    }

    public async Task<(ValidationReport report, Organizer? organizer)> ValidateForDelete(int organizerId)
    {
        var report = new ValidationReport();
        var organizer = await _repository.Get(organizerId);

        if (organizer is null)
        {
            report.Success = false;
            report.Message = "Organizer not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Organizer found and marked for delete";
        return (report, organizer);
    }
}

