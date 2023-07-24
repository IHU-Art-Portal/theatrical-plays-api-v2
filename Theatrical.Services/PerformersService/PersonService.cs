using Theatrical.Data.Models;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.PersonDtos;
using Theatrical.Services.Pagination;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.PerformersService;

public interface IPersonService
{
    Task Create(CreatePersonDto createPersonDto);
    Task<PaginationResult<PersonDto>> GetAndPaginate(int? page, int? size);
    Task Delete(Person person);
    PersonDto ToDto(Person person);
    PaginationResult<PersonDto> PaginateAndProduceDtos(List<Person> persons, int? page, int? size);

    PaginationResult<PersonProductionsRoleInfo> PaginateContributionsOfPerson(
        List<PersonProductionsRoleInfo> personProductionsRole, int? page, int? size);

    List<ImageDto> ImagesToDto(List<Image> images);

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

    public async Task Create(CreatePersonDto createPersonDto)
    {
        Person person = new Person
        {
            Fullname = createPersonDto.Fullname,
            Timestamp = DateTime.UtcNow,
            SystemId = createPersonDto.System
        };

        if (createPersonDto.Images != null && createPersonDto.Images.Any())
        {
            List<Image> images = new List<Image>();
            
            foreach (string imageUrl in createPersonDto.Images)
            {
                Image image = new Image { ImageUrl = imageUrl };
                images.Add(image);
            }

            person.Images = images;
        }
        
        
        await _repository.Create(person);
    }

    public async Task<PaginationResult<PersonDto>> GetAndPaginate(int? page, int? size)
    {
        List<Person> persons = await _repository.Get();

        var paginationResult = _pagination.GetPaginated(page, size, persons, items =>
        {
            return items.Select(personsDto => new PersonDto
            {
                Id = personsDto.Id,
                Fullname = personsDto.Fullname,
                SystemID = personsDto.SystemId
            });
        });
        
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
            SystemID = person.SystemId
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
                SystemID = personsDto.SystemId
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
}