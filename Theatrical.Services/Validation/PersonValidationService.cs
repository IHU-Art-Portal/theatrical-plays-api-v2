using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IPersonValidationService
{
    Task<(ValidationReport report, Person? person)> ValidateAndFetch(int performerId);
    Task<(ValidationReport report, Person? person)> ValidateForDelete(int performerId);
    Task<(ValidationReport report, List<Person>? person)> ValidateForFetchRole(string role);
}

public class PersonValidationService : IPersonValidationService
{
    private readonly IPersonRepository _repository;

    public PersonValidationService(IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<(ValidationReport report, Person? person)> ValidateAndFetch(int performerId)
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

    public async Task<(ValidationReport report, Person? person)> ValidateForDelete(int performerId)
    {
        var person = await _repository.Get(performerId);
        var report = new ValidationReport();
        
        if (person is null)
        {
            report.Success = false;
            report.Message = "Person not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Person exists and marked for deletion";
        return (report, person);
    }

    public async Task<(ValidationReport report, List<Person>? person)> ValidateForFetchRole(string role)
    {
        var persons = await _repository.GetByRole(role);
        var report = new ValidationReport();

        if (!persons.Any())
        {
            report.Success = false;
            report.Message = "Not Found any people with the provided role";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Found people with the provided role";
        return (report, persons);

    }
}