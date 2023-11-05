using System.Globalization;
using Theatrical.Data.Models;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.PersonDtos;
using Theatrical.Services.Pagination;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.PerformersService;

public interface IPersonService
{
    Task<Person> Create(CreatePersonDto createPersonDto);
    Task<PaginationResult<PersonDto>> GetAndPaginate(int? page, int? size, bool? showAvailableAccounts);
    Task Delete(Person person);
    PersonDto ToDto(Person person);
    PaginationResult<PersonDto> PaginateAndProduceDtos(List<Person> persons, int? page, int? size);

    PaginationResult<PersonProductionsRoleInfo> PaginateContributionsOfPerson(
        List<PersonProductionsRoleInfo> personProductionsRole, int? page, int? size);

    List<ImageDto> ImagesToDto(List<Image> images);
    Task<List<Image>?> GetImages();
    Task DeleteTestData();
    Task CreateList(List<CreatePersonDto> addingPeople);
}

public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;
    private readonly IPaginationService _pagination;
    private readonly IAccountRequestRepository _accountRequestRepository;

    public PersonService(IPersonRepository repository, IPaginationService paginationService, IAccountRequestRepository accountRequestRepository)
    {
        _repository = repository;
        _pagination = paginationService;
        _accountRequestRepository = accountRequestRepository;
    }

    public async Task<Person> Create(CreatePersonDto createPersonDto)
    {
        Person person = new Person
        {
            Fullname = createPersonDto.Fullname,
            Timestamp = DateTime.UtcNow,
            SystemId = createPersonDto.System,
            HairColor = createPersonDto.HairColor,
            Height = createPersonDto.Height,
            EyeColor = createPersonDto.EyeColor,
            Weight = createPersonDto.Weight,
            Languages = createPersonDto.Languages,
            Description = createPersonDto.Description,
            Bio = createPersonDto.Bio,
            Roles = createPersonDto.Roles
        };

        if (createPersonDto.Birthdate is not null)
        {
            const string format = "dd-MM-yyyy";
            try
            {
                DateTime parsedDate = DateTime.ParseExact(createPersonDto.Birthdate, format, CultureInfo.InvariantCulture);

                // Explicitly set the DateTimeKind to UTC, to avoid errors.
                person.Birthdate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        if (createPersonDto.Images != null && createPersonDto.Images.Any())
        {
            List<Image> images = createPersonDto.Images.Select(imageUrl => new Image { ImageUrl = imageUrl }).ToList();

            person.Images = images;
        }
        
        var createdPerson = await _repository.Create(person);
        return createdPerson;
    }

    /// <summary>
    /// ShowAvailableAccounts is a parameter that changed the flow of code.
    /// If left empty,
    ///     the method retrieves all people,
    ///     paginates the list and sends it back.
    /// If true,
    ///     returns all people that are available for claiming from a user,
    ///     also paginates the result.
    /// If false,
    ///     returns already claimed accounts,
    ///     also paginates the result.
    /// Pagination works only if page and/or size is defined.
    /// Pagination Behavior:
    ///           only page: returns the specified page, with 10 results per page,
    ///           only size: returns always the 1st page, with specified sized results.
    /// </summary>
    /// <param name="page"> integer (optional) </param>
    /// <param name="size"> integer (optional) </param>
    /// <param name="showAvailableAccounts"> boolean (optional)</param>
    /// <returns></returns>
    public async Task<PaginationResult<PersonDto>> GetAndPaginate(int? page, int? size, bool? showAvailableAccounts)
    {
        if (showAvailableAccounts is true)
        {
            var availableAccounts = await _repository.GetPeopleByClaimingStatus(ClaimingStatus.Available);
            var paginationResult1 = PaginateAndProduceDtos(availableAccounts, page, size);
            return paginationResult1;
        }

        if (showAvailableAccounts is false)
        {
            var nonAvailableAccounts = await _repository.GetPeopleByClaimingStatus(ClaimingStatus.Unavailable);
            var paginationResult2 = PaginateAndProduceDtos(nonAvailableAccounts, page, size);
            return paginationResult2;
        }
        
        //Normal flow if `showAvailableAccounts` is empty/null;
        List<Person> persons = await _repository.Get();
        
        var paginationResult = PaginateAndProduceDtos(persons, page, size);
        
        return paginationResult;
    }

    public async Task Delete(Person person)
    {
        await _repository.Delete(person);
    }

    public PersonDto ToDto(Person person)
    {
        var personDto = new PersonDto
        {
            Id = person.Id,
            Fullname = person.Fullname,
            SystemID = person.SystemId,
            Bio = person.Bio,
            Birthdate = person.Birthdate != null ? person.Birthdate.ToString() : null, // Conditionally add Birthdate
            Description = person.Description,
            Languages = person.Languages,
            Weight = person.Weight,
            Height = person.Height,
            EyeColor = person.EyeColor,
            HairColor = person.HairColor,
            Roles = person.Roles
        };
        return personDto;
    }

    public PaginationResult<PersonDto> PaginateAndProduceDtos(List<Person> persons, int? page, int? size)
    {
        var paginationResult = _pagination.GetPaginated(page, size, persons, items =>
        {
            return items.Select(personsDto => new PersonDto
            {
                Id = personsDto.Id,
                Fullname = personsDto.Fullname,
                SystemID = personsDto.SystemId,
                Bio = personsDto.Bio,
                Birthdate = personsDto.Birthdate != null ? personsDto.Birthdate.ToString() : null, // Conditionally add Birthdate
                Description = personsDto.Description,
                Languages = personsDto.Languages,
                Weight = personsDto.Weight,
                Height = personsDto.Height,
                EyeColor = personsDto.Height,
                HairColor = personsDto.HairColor,
                Roles = personsDto.Roles
            });
        });
        
        return paginationResult;
    }

    public PaginationResult<PersonProductionsRoleInfo> PaginateContributionsOfPerson(List<PersonProductionsRoleInfo> personProductionsRole, int? page, int? size)
    {
        var paginationResult = _pagination.GetPaginated(page, size, personProductionsRole, items =>
        {
            return items.Select(personsProductionsDto => new PersonProductionsRoleInfo
            {
                Production = personsProductionsDto.Production,
                Role = personsProductionsDto.Role
            });
        });

        return paginationResult;
    }

    public List<ImageDto> ImagesToDto(List<Image> images)
    {
        return images.Select(image => new ImageDto { Id = image.Id, ImageUrl = image.ImageUrl, PersonId = image.PersonId }).ToList();
    }

    public async Task<List<Image>?> GetImages()
    {
        var images = await _repository.GetImages();
        return images;
    }

    public async Task DeleteTestData()
    {
        await _repository.DeleteTestData();
    }

    public async Task CreateList(List<CreatePersonDto> addingPeople)
    {
        var finalPeopleToAdd = new List<Person>();
        foreach (var personDto in addingPeople)
        {
            var person = new Person
            {
                Fullname = personDto.Fullname,
                Timestamp = DateTime.UtcNow,
                SystemId = personDto.System,
                HairColor = personDto.HairColor,
                Height = personDto.Height,
                EyeColor = personDto.EyeColor,
                Weight = personDto.Weight,
                Languages = personDto.Languages,
                Description = personDto.Description,
                Bio = personDto.Bio,
                Roles = personDto.Roles
            };

            if (personDto.Birthdate is not null)
            {
                const string format = "dd-MM-yyyy";
                try
                {
                    DateTime parsedDate = DateTime.ParseExact(personDto.Birthdate, format, CultureInfo.InvariantCulture);

                    // Explicitly set the DateTimeKind to UTC, to avoid errors.
                    person.Birthdate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (personDto.Images != null && personDto.Images.Any())
            {
                List<Image> images = personDto.Images.Select(imageUrl => new Image { ImageUrl = imageUrl }).ToList();

                person.Images = images;
            }
        
            finalPeopleToAdd.Add(person);
        }

        await _repository.CreateRange(finalPeopleToAdd);
    }
}