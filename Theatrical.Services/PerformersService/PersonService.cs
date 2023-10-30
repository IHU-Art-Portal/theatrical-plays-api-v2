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
    Task<PaginationResult<PersonDto>> GetAndPaginate(int? page, int? size);
    Task Delete(Person person);
    PersonDto ToDto(Person person);
    PaginationResult<PersonDto> PaginateAndProduceDtos(List<Person> persons, int? page, int? size);

    PaginationResult<PersonProductionsRoleInfo> PaginateContributionsOfPerson(
        List<PersonProductionsRoleInfo> personProductionsRole, int? page, int? size);

    List<ImageDto> ImagesToDto(List<Image> images);
    Task<List<Image>?> GetImages();

}

public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;
    private readonly IPaginationService _pagination;

    public PersonService(IPersonRepository repository, IPaginationService paginationService)
    {
        _repository = repository;
        _pagination = paginationService;
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

    //Retrieves all persons.
    //Calls Pagination Method.
    public async Task<PaginationResult<PersonDto>> GetAndPaginate(int? page, int? size)
    {
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
            EyeColor = person.Height,
            HairColor = person.HairColor
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
                HairColor = personsDto.HairColor
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
}