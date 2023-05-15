using Theatrical.Data.Models;
using Theatrical.Dto.OrganizerDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IOrganizerService
{
    Task<List<Organizer?>> Get();
    Task Create(OrganizerCreateDto organizerCreateDto);
    Task Delete(Organizer organizer);
}

public class OrganizerService : IOrganizerService
{
    private readonly IOrganizerRepository _repository;

    public OrganizerService(IOrganizerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Organizer?>> Get()
    {
        var organizers = await _repository.Get();
        return organizers;
    }

    public async Task Create(OrganizerCreateDto organizerCreateDto)
    {
        var organizer = new Organizer
        {
            Name = organizerCreateDto.Name,
            Address = organizerCreateDto.Address,
            Town = organizerCreateDto.Town,
            Phone = organizerCreateDto.Phone,
            Email = organizerCreateDto.Email,
            Doy = organizerCreateDto.Doy,
            Afm = organizerCreateDto.Afm,
            Created = DateTime.UtcNow
        };
        await _repository.Create(organizer);
    }

    public async Task Delete(Organizer organizer)
    {
        await _repository.Delete(organizer);
    }
}

