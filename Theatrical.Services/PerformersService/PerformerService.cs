using Theatrical.Data.Models;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.PerformersService;

public interface IPerformerService
{
    Task Create(CreatePerformerDto createPerformerDto);
    Task<PerformersPaginationDto> Get(int? page, int? size);
    Task Delete(Performer performer);
    Task<PerformerDto> Get(int id);
}

public class PerformerService : IPerformerService
{
    private readonly IPerformerRepository _repository;

    public PerformerService(IPerformerRepository repository)
    {
        _repository = repository;
    }

    public async Task Create(CreatePerformerDto createPerformerDto)
    {
        Performer performer = new Performer
        {
            Name = createPerformerDto.Name,
            Surname = createPerformerDto.Surname,
            Created = DateTime.UtcNow
        };
        await _repository.Create(performer);
    }

    public async Task<PerformersPaginationDto> Get(int? page, int? size)
    {
        List<Performer> performers = await _repository.Get();
        List<PerformerDto> performerDtos = new();
        
        
        if (page is null && size is null)
        {
            performerDtos.AddRange(performers.Select(performer => 
                new PerformerDto
                {
                    Id = performer.Id,
                    Name = performer.Name,
                    Surname = performer.Surname
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
            PerformerDto performerDto = new PerformerDto
            {
                Id = performer.Id,
                Name = performer.Name,
                Surname = performer.Surname
            };
            performerDtos.Add(performerDto);
        }

        PerformersPaginationDto response1 = new PerformersPaginationDto
        {
            Performers = performerDtos,
            CurrentPage = page,
            PageSize = (int)pageCount
        };
       

        return response1;
    }

    public async Task Delete(Performer performer)
    {
        await _repository.Delete(performer);
    }

    public async Task<PerformerDto> Get(int id)
    {
        var performer = await _repository.Get(id);
        PerformerDto performerDto = new PerformerDto
        {
            Id = performer.Id,
            Name = performer.Name,
            Surname = performer.Surname
        };
        return performerDto;
    }

}