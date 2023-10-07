using Theatrical.Data.Models;
using Theatrical.Dto.ContributionDtos;
using Theatrical.Dto.Pagination;
using Theatrical.Services.Pagination;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;


public interface IContributionService
{
    Task<Contribution> Create(CreateContributionDto createContributionDto);
    PaginationResult<ContributionDto> Paginate(int? page, int? size, List<ContributionDto> contributionDtos);
    List<ContributionDto> ToDto(List<Contribution> contributions);
    ContributionDto ToDto(Contribution contr);
}

public class ContributionService : IContributionService
{
    private readonly IContributionRepository _repository;
    private readonly IPaginationService _pagination;

    public ContributionService(IContributionRepository repository, IPaginationService paginationService)
    {
        _repository = repository;
        _pagination = paginationService;
    }

    public async Task<Contribution> Create(CreateContributionDto createContributionDto)
    {
        Contribution contribution = new Contribution
        {
            PeopleId = createContributionDto.PeopleId,
            ProductionId = createContributionDto.ProductionId,
            RoleId = createContributionDto.RoleId,
            SubRole = createContributionDto.SubRole,
            Timestamp = DateTime.UtcNow
        };

        var createdContribution = await _repository.Create(contribution);
        return createdContribution;
    }

    public List<ContributionDto> ToDto(List<Contribution> contributions)
    {
        return contributions.Select(contr => new ContributionDto
        {
            Id = contr.Id,
            PeopleId = contr.PeopleId,
            ProductionId = contr.ProductionId,
            RoleId = contr.RoleId,
            SubRole = contr.SubRole,
            SystemId = contr.SystemId,
            Timestamp = contr.Timestamp
        }).ToList();
    }

    public PaginationResult<ContributionDto> Paginate(int? page, int? size, List<ContributionDto> contributionDtos)
    {
        var paginationResult = _pagination.GetPaginated(page, size, contributionDtos, items =>
        {
            return items.Select(contr => new ContributionDto
            {
                Id = contr.Id,
                PeopleId = contr.PeopleId,
                ProductionId = contr.ProductionId,
                RoleId = contr.RoleId,
                SubRole = contr.SubRole,
                SystemId = contr.SystemId,
                Timestamp = contr.Timestamp
            });
        });

        return paginationResult;
    }
    
    public ContributionDto ToDto(Contribution contr)
    {
        return (new ContributionDto
        {
            Id = contr.Id,
            PeopleId = contr.PeopleId,
            ProductionId = contr.ProductionId,
            RoleId = contr.RoleId,
            SubRole = contr.SubRole,
            SystemId = contr.SystemId,
            Timestamp = contr.Timestamp
        });
    }
}

