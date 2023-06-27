using Theatrical.Data.Models;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.PerformersService;

public interface IPersonService
{
    Task Create(CreatePerformerDto createPerformerDto);
    Task<PerformersPaginationDto> Get(int? page, int? size);
    Task Delete(Person person);
    Task<PersonDto> Get(Person person);
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
        List<Person> performers = await _repository.Get();
        List<PersonDto> performerDtos = new();
        
        if (page is null && size is null)
        {
            performerDtos.AddRange(performers.Select(performer => 
                new PersonDto
                {
                    Id = performer.Id,
                    Fullname = performer.Fullname
                }));

            var response = new PerformersPaginationDto
            {
                Performers = performerDtos,
                CurrentPage = null,
                PageSize = null
            };

            return response;
        }

        size ??= 10;
        if (page is null) page = 1;

        var pageResults = (float)size;
        var pageCount = Math.Ceiling(performers.Count / pageResults);

        var performersPaged = performers
            .Skip((page.Value - 1) * (int)pageResults)
            .Take((int)pageResults)
            .ToList();

        foreach (var performer in performersPaged)
        {
            PersonDto personDto = new PersonDto
            {
                Id = performer.Id,
                Fullname = performer.Fullname
            };
            performerDtos.Add(personDto);
        }

        PerformersPaginationDto response1 = new PerformersPaginationDto
        {
            Performers = performerDtos,
            CurrentPage = page,
            PageSize = (int)pageCount
        };
       

        return response1;
    }

    public async Task Delete(Person person)
    {
        await _repository.Delete(person);
    }

    public async Task<PersonDto> Get(Person person)
    {
        var performerDto = new PersonDto
        {
            Id = person.Id,
            Fullname = person.Fullname
        };
        return performerDto;
    }

}