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
    Task<List<Organizer>> GetOrganizersByNames(List<string> organizersNames);
    Task<(List<Organizer> organizersToUpdate, List<Organizer> organizersToCreate, List<OrganizerDto> responseList)> CreateListsAndUpdateCreate(List<Organizer> existingOrganizers, List<OrganizerCreateDto> organizerCreateDtos);
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
            Postcode = organizerCreateDto.Postcode,
            Timestamp = DateTime.UtcNow,
            SystemId = organizerCreateDto.SystemId
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
                Town = org.Town
            });
        });

        return paginationResult;
    }

    public async Task<List<Organizer>> GetOrganizersByNames(List<string> organizersNames)
    {
        return await _repository.GetOrganizersByNames(organizersNames);
    }

    public async Task<(List<Organizer> organizersToUpdate, List<Organizer> organizersToCreate, List<OrganizerDto> responseList)> CreateListsAndUpdateCreate(List<Organizer> existingOrganizers, List<OrganizerCreateDto> organizerCreateDtos)
    {
        var organizersToUpdate = new List<Organizer>();
        var organizersToCreate = new List<Organizer>();
        var responseList = new List<OrganizerDto>();

        foreach (var dto in organizerCreateDtos)
        {
            var existingOrganizer = existingOrganizers.FirstOrDefault(v => v.Name == dto.Name);

            if (existingOrganizer is not null)
            {
                existingOrganizer.Name = dto.Name;
                if (dto.Address != null) existingOrganizer.Address = dto.Address;
                if (dto.Town != null) existingOrganizer.Town = dto.Town;
                if (dto.Postcode != null) existingOrganizer.Postcode = dto.Postcode;
                if (dto.Phone != null) existingOrganizer.Phone = dto.Phone;
                if (dto.Email != null) existingOrganizer.Email = dto.Email;
                if (dto.Doy != null) existingOrganizer.Doy = dto.Doy;
                if (dto.Afm != null) existingOrganizer.Afm = dto.Afm;
                existingOrganizer.SystemId = dto.SystemId;

                organizersToUpdate.Add(existingOrganizer);
            }
            else
            {
                var newOrganizer = new Organizer
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    Town = dto.Town,
                    Postcode = dto.Postcode,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    Doy = dto.Doy,
                    Afm = dto.Afm,
                    SystemId = dto.SystemId
                };
                organizersToCreate.Add(newOrganizer);
            }
        }
        
        //Saves changes for all updated venues
        if (organizersToUpdate.Any())
        {
            var updatedOrganizers = await _repository.UpdateRange(organizersToUpdate);
            responseList.AddRange(ToDto(updatedOrganizers));
        }
        
        //Creates venues
        if (organizersToCreate.Any())
        {
            var createdVenues = await _repository.CreateRange(organizersToCreate);
            responseList.AddRange(ToDto(createdVenues));
        }

        return (organizersToUpdate, organizersToCreate, responseList);
    }
}

