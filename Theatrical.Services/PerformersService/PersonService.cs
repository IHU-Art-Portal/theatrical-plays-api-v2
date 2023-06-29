using Theatrical.Data.Models;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.PerformersService;

public interface IPersonService
{
    Task Create(CreatePerformerDto createPerformerDto);
    Task<PerformersPaginationDto> Get(int? page, int? size);
    Task Delete(Person person);
    PersonDto ToDto(Person person);
}

public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;

    public PersonService(IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task Create(CreatePerformerDto createPerformerDto)
    {
        Person person = new Person
        {
            Fullname = createPerformerDto.Fullname,
            Timestamp = DateTime.UtcNow
        };

        if (createPerformerDto.Images != null && createPerformerDto.Images.Any())
        {
            List<Image> images = new List<Image>();
            
            foreach (string imageUrl in createPerformerDto.Images)
            {
                Image image = new Image { ImageUrl = imageUrl };
                images.Add(image);
            }

            person.Images = images;
        }
        
        
        await _repository.Create(person);
    }

    public async Task<PerformersPaginationDto> Get(int? page, int? size)
    {
        List<Person> persons = await _repository.Get();
        List<PersonDto> personDtos = new();
        
        if (page is null && size is null)
        {
            personDtos.AddRange(persons.Select(person => 
                new PersonDto
                {
                    Id = person.Id,
                    Fullname = person.Fullname,
                    SystemID = person.SystemId
                }));

            var response = new PerformersPaginationDto
            {
                Persons = personDtos,
                CurrentPage = null,
                PageSize = null
            };

            return response;
        }

        size ??= 10;
        if (page is null) page = 1;

        var pageResults = (float)size;
        var pageCount = Math.Ceiling(persons.Count / pageResults);

        var personsPaged = persons
            .Skip((page.Value - 1) * (int)pageResults)
            .Take((int)pageResults)
            .ToList();

        foreach (var person in personsPaged)
        {
            PersonDto personDto = new PersonDto
            {
                Id = person.Id,
                Fullname = person.Fullname,
                SystemID = person.SystemId
            };
            personDtos.Add(personDto);
        }

        PerformersPaginationDto response1 = new PerformersPaginationDto
        {
            Persons = personDtos,
            CurrentPage = page,
            PageSize = (int)pageCount
        };
       

        return response1;
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

}