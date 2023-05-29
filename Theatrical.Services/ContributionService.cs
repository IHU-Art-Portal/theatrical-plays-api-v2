using Theatrical.Data.Models;
using Theatrical.Dto.ContributionDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;


public interface IContributionService
{
    Task Create(CreateContributionDto createContributionDto);
}

public class ContributionService : IContributionService
{
    private readonly IContributionRepository _repository;

    public ContributionService(IContributionRepository repository)
    {
        _repository = repository;
    }

    public async Task Create(CreateContributionDto createContributionDto)
    {
        Contribution contribution = new Contribution
        {
            PerformerId = createContributionDto.PerformerId,
            ProductionId = createContributionDto.ProductionId,
            RoleId = createContributionDto.RoleId,
            Subrole = createContributionDto.SubRole,
            Created = DateTime.UtcNow
        };

        await _repository.Create(contribution);
    }
}

