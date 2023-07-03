using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Dto.PersonDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IPersonValidationService
{
    Task<(ValidationReport report, Person? person)> ValidateAndFetch(int performerId);
    Task<(ValidationReport report, Person? person)> ValidateForDelete(int performerId);
    Task<(ValidationReport report, List<Person>? person)> ValidateForFetchRole(string role);
    Task<(ValidationReport report, List<Person>? person)> ValidateForInitials(string initials);
    Task<(ValidationReport report, List<PersonProductionsRoleInfo>? productions)>
        ValidatePersonsProductions(int personId);
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
        var person = await _repository.Get(performerId);
        var report = new ValidationReport();

        if (person is null)
        {
            report.Success = false;
            report.Message = "Person not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Person exists";
        return (report, person);
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

    public async Task<(ValidationReport report, List<Person>? person)> ValidateForInitials(string initials)
    {
        var persons = await _repository.GetByLetter(initials);
        var report = new ValidationReport();

        if (!persons.Any())
        {
            report.Success = false;
            report.Message = $"Not Found any people with the initials: {initials}";
            return (report, null);
        }
        
        report.Success = true;
        report.Message = "Found people with the provided criteria";
        return (report, persons);
    }

    public async Task<(ValidationReport report, List<PersonProductionsRoleInfo>? productions)> ValidatePersonsProductions(int personId)
    {
        var (report, person) = await ValidateAndFetch(personId);

        if (!report.Success)
        {
            report.Message = $"Person with the provided ID not found";
            return (report, null);
        }

        var productions = await _repository.GetProductionsOfPerson(person!.Id);

        if (!productions.Any())
        {
            report.Message = $"This person does not participate in any productions";
            return (report, null);
        }

        report.Message = "Successful";
        report.Success = true;

        return (report, productions);
    }
}