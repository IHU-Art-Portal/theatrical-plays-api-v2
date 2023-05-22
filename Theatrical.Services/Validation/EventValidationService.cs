using Theatrical.Data.Models;
using Theatrical.Dto.EventDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IEventValidationService
{
    Task<ValidationReport> ValidateForCreate(CreateEventDto createEventDto);
}

public class EventValidationService : IEventValidationService
{
    private readonly IEventRepository _repository;
    private readonly IProductionRepository _productionRepo;
    private readonly IVenueRepository _venueRepo;

    public EventValidationService(IEventRepository repository, IProductionRepository productionRepository, IVenueRepository venueRepository)
    {
        _repository = repository;
        _productionRepo = productionRepository;
        _venueRepo = venueRepository;
    }

    public async Task<ValidationReport> ValidateForCreate(CreateEventDto createEventDto)
    {
        var report = new ValidationReport();
        var production = await _productionRepo.GetProduction(createEventDto.ProductionId);
        
        if (production is null)
        {
            report.Success = false;
            report.Message = $"Production with ID: {createEventDto.ProductionId} does not exist";
            return report;
        }

        var venue = await _venueRepo.Get(createEventDto.VenueId);

        if (venue is null)
        {
            report.Success = false;
            report.Message = $"Venue with Id: {createEventDto.VenueId} does not exist";
            return report;
        }

        report.Success = true;
        report.Message = "Success";
        return report;
    }
}

