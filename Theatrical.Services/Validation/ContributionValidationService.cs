using Theatrical.Data.Models;
using Theatrical.Dto.ContributionDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IContributionValidationService
{
    Task<(ValidationReport report, List<Contribution> contributions)> ValidateForFetch();
    Task<ValidationReport> ValidateForCreate(CreateContributionDto contributionDto);
}

public class ContributionValidationService : IContributionValidationService
{
    private readonly IContributionRepository _repository;

    public ContributionValidationService(IContributionRepository repository)
    {
        _repository = repository;
    }

    public async Task<(ValidationReport report, List<Contribution> contributions)> ValidateForFetch()
    {
        var report = new ValidationReport();
        var contributions = await _repository.Get();

        if (!contributions.Any())
        {
            report.Success = false;
            report.Message = "Not found any Contributions";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Contributions Exist";
        
        return (report, contributions);
    }
    
    public async Task<ValidationReport> ValidateForCreate(CreateContributionDto contributionDto)
    {
        var report = new ValidationReport();
        var existence = await _repository.CheckExists(contributionDto.PeopleId, contributionDto.ProductionId, contributionDto.RoleId);

        if (!existence.performerExists)
        {
            report.Success = false;
            report.Message = $"Performer with Id: {contributionDto.PeopleId} not found";
            return report;
        }

        if (!existence.productionExists)
        {
            report.Success = false;
            report.Message = $"Production with Id: {contributionDto.ProductionId} not found";
            return report;
        }

        if (!existence.roleExists)
        {
            report.Success = false;
            report.Message = $"Role with Id: {contributionDto.RoleId} not found";
            return report;
        }

        report.Success = true;
        report.Message = "This contribution can be created";
        return report;
    }
}

