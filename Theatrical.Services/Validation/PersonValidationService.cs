using Theatrical.Data.Models;
using Theatrical.Dto.PersonDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Curators.DataCreationCurators;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IPersonValidationService
{
    Task<(ValidationReport report, Person? person)> ValidateAndFetch(int performerId);
    Task<(ValidationReport report, Person? person)> ValidateForDelete(int performerId);
    Task<(ValidationReport report, List<Person>? person)> ValidateForFetchRole(string role);
    Task<(ValidationReport report, List<Person>? person)> ValidateForInitials(string initials);
    Task<(ValidationReport report, List<PersonProductionsRoleInfo>? productions)> ValidatePersonsProductions(int personId);
    Task<(ValidationReport report, List<Image>? images)> ValidatePersonsPhotos(int personId);
    Task<(ValidationReport, CreatePersonDto?)> ValidateForCreate(CreatePersonDto createPersonDto);
    Task<User?> ValidateWithEmail(string email);
    Task<(List<Person>?, List<CreatePersonDto>?)> ValidateForCreateList(List<CreatePersonDto> createPeopleDto);
}

public class PersonValidationService : IPersonValidationService
{
    private readonly IPersonRepository _repository;
    private readonly ICuratorIncomingData _curatorIncomingData;
    private readonly IUserRepository _userRepository;

    public PersonValidationService(IPersonRepository repository, ICuratorIncomingData curatorIncomingData, IUserRepository userRepository)
    {
        _repository = repository;
        _curatorIncomingData = curatorIncomingData;
        _userRepository = userRepository;
    }

    public async Task<(ValidationReport report, Person? person)> ValidateAndFetch(int performerId)
    {
        var person = await _repository.Get(performerId);
        var report = new ValidationReport();

        if (person is null)
        {
            report.Success = false;
            report.Message = "Person not found";
            report.ErrorCode = ErrorCode.NotFound;
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
            report.ErrorCode = ErrorCode.NotFound;
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
            report.ErrorCode = ErrorCode.NotFound;
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
            report.ErrorCode = ErrorCode.NotFound;
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
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        var productions = await _repository.GetProductionsOfPerson(person!.Id);

        if (!productions.Any())
        {
            report.Message = $"This person does not participate in any productions";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Message = "Successful";
        report.Success = true;

        return (report, productions);
    }

    public async Task<(ValidationReport report, List<Image>? images)> ValidatePersonsPhotos(int personId)
    {
        var person = await _repository.Get(personId);
        var report = new ValidationReport();
        
        if (person is null)
        {
            report.Success = false;
            report.Message = "Person not found";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        var images = await _repository.GetPersonsImages(personId);

        report.Success = true;
        report.Message = "Successful";

        return (report, images);
    }

    public async Task<(ValidationReport, CreatePersonDto?)> ValidateForCreate(CreatePersonDto createPersonDto)
    {
        var report = new ValidationReport();
        
        _curatorIncomingData.CorrectString(createPersonDto.Fullname);      //Corrects the name before checking the db.
        var person = await _repository.GetByName(createPersonDto.Fullname);

        if (person is not null)
        {
            report.Success = false;
            report.Message = "Person with a similar name already exists.";
            report.ErrorCode = ErrorCode.AlreadyExists;
            return (report, null);
        }
        
        _curatorIncomingData.CorrectIncomingPerson(createPersonDto);    //Corrects other fields afterwards.

        report.Message = "Success";
        report.Success = true;
        return (report, createPersonDto);
    }

    public async Task<User?> ValidateWithEmail(string email)
    {
        var user = await _userRepository.Get(email);
        return user;
    }
    
    public async Task<(List<Person>?, List<CreatePersonDto>?)> ValidateForCreateList(List<CreatePersonDto> createPeopleDto)
    {

        foreach (var personDto in createPeopleDto)
        {
            _curatorIncomingData.CorrectString(personDto.Fullname);
        }

        //people that were found in database, and should be updated.
        var peopleMatched = await _repository.GetByNameRange(createPeopleDto);

        if (peopleMatched is not null && peopleMatched.Count > 0)
        {
            //people that were not found in database, and should be added.
            List<CreatePersonDto> peopleNotMatched = createPeopleDto                          
                .Where(dto => !peopleMatched.Any(p => p.Fullname == dto.Fullname))
                .ToList();
            
            foreach (var personDto in peopleNotMatched)
            {
                _curatorIncomingData.CorrectIncomingPerson(personDto);
            }
        
            return (peopleMatched, peopleNotMatched);
        }

        foreach (var personDto in createPeopleDto)
        {
            _curatorIncomingData.CorrectIncomingPerson(personDto);
        }
        
        return (peopleMatched, createPeopleDto);
    }
}