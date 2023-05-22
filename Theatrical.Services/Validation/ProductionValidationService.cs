using Theatrical.Data.Models;
using Theatrical.Dto.ProductionDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IProductionValidationService
{
    Task<(ValidationReport report, List<Production> productions)> ValidateAndFetch();
    Task<ValidationReport> ValidateForCreate(CreateProductionDto productionDto);
    Task<(ValidationReport report, Production production)> ValidateForDelete(int productionId);
}

public class ProductionValidationService : IProductionValidationService
{
    private readonly IProductionRepository _repository;
    private readonly IOrganizerRepository _organizerRepo;

    public ProductionValidationService(IProductionRepository repository, IOrganizerRepository organizerRepo)
    {
        _repository = repository;
        _organizerRepo = organizerRepo;
    }

    public async Task<ValidationReport> ValidateForCreate(CreateProductionDto productionDto)
    {
        var organizer = await _organizerRepo.Get(productionDto.OrganizerId);
        var report = new ValidationReport();
        
        if (organizer is null)
        {
            report.Success = false;
            report.Message = "Organizer not found. Please select a valid organizer's id";
            return report;
        }

        report.Message = "Organizer found. Proceeding to creating the Production";
        report.Success = true;

        return report;
    }
    
    public async Task<(ValidationReport report, List<Production> productions)> ValidateAndFetch()
    {
        var productions = await _repository.Get();
        var report = new ValidationReport();
        
        if (!productions.Any())
        {
            report.Message = "Not found any productions";
            report.Success = false;
            return (report, null);
        }

        report.Message = "Productions found";
        report.Success = true;

        return (report, productions);
    }

    public async Task<(ValidationReport report, Production production)> ValidateForDelete(int productionId)
    {
        var production = await _repository.GetProduction(productionId);
        var report = new ValidationReport();

        if (production is null)
        {
            report.Success = false;
            report.Message = "Production Not Found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Production Found!";
        return (report, production);

    }
    
}