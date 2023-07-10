using Theatrical.Data.Models;
using Theatrical.Dto.OrganizerDtos;
using Theatrical.Dto.Pagination;
using Theatrical.Services.Pagination;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IOrganizerService
{
    Task<List<Organizer?>> Get();
    Task Create(OrganizerCreateDto organizerCreateDto);
    Task Delete(Organizer organizer);
    List<OrganizerDto> ToDto(List<Organizer> organizers);
    PaginationResult<OrganizerDto> Paginate(int? page, int? size, List<OrganizerDto> organizerDtos);
}

public class OrganizerService : IOrganizerService
{
    private readonly IOrganizerRepository _repository;
    private readonly IPaginationService _pagination;

    public OrganizerService(IOrganizerRepository repository, IPaginationService paginationService)
    {
        _repository = repository;
        _pagination = paginationService;
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
            Timestamp = DateTime.UtcNow
        };
        await _repository.Create(organizer);
    }

    public async Task Delete(Organizer organizer)
    {
        await _repository.Delete(organizer);
    }

    public List<OrganizerDto> ToDto(List<Organizer> organizers)
    {
        return organizers.Select(organizer => new OrganizerDto
        {
            Address = organizer.Address,
            Afm = organizer.Afm,
            Doy = organizer.Doy,
            Email = organizer.Email,
            Id = organizer.Id,
            Name = organizer.Name,
            Phone = organizer.Phone,
            Postcode = organizer.Postcode,
            SystemId = organizer.SystemId,
            Timestamp = organizer.Timestamp,
            Town = organizer.Town
        }).ToList();
    }
    
    public PaginationResult<OrganizerDto> Paginate(int? page, int? size, List<OrganizerDto> organizerDtos)
    {
        var paginationResult = _pagination.GetPaginated(page, size, organizerDtos, items =>
        {
            return items.Select(org => new OrganizerDto
            {
                Address = org.Address,
                Afm = org.Afm,
                Doy = org.Doy,
                Email = org.Email,
                Id = org.Id,
                Name = org.Name,
                Phone = org.Phone,
                Postcode = org.Postcode,
                SystemId = org.SystemId,
                Timestamp = org.Timestamp,
                Town = org.Town
            });
        });

        return paginationResult;
    }
}

