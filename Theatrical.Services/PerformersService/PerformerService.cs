using Theatrical.Data.Models;
using Theatrical.Dto.PerformerDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.PerformersService;

public interface IPerformerService
{
    Task Create(CreatePerformerDto createPerformerDto);
    Task<PerformersPaginationDto> Get();
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

    public async Task<PerformersPaginationDto> Get()
    {
        List<Performer> performers = await _repository.Get();
        List<PerformerDto> performerDtos = new();

        foreach (var performer in performers)
        {
            var performerDto = new PerformerDto
            {
                Id = performer.Id,
                Name = performer.Name,
                Surname = performer.Surname
            };
            performerDtos.Add(performerDto);
        }

        PerformersPaginationDto performersPaginationDto = new PerformersPaginationDto
        {
            Performers = performerDtos,
            CurrentPage = null,
            PageSize = null
        };
        
        return performersPaginationDto;
    }
}