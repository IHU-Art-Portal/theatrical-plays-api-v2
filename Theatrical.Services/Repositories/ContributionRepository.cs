using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Services.Caching;

namespace Theatrical.Services.Repositories;

public interface IContributionRepository
{
    Task<List<Contribution>> Get();
    Task<Contribution> Create(Contribution contribution);
    Task<List<Contribution>> GetSpecific(int personId, int productionId, int roleId);
    Task<List<Contribution>> GetByRole(int roleId);
    Task<List<Contribution>> GetByProduction(int productionId);
    Task<List<Contribution>> GetByPerformer(int personId);

    Task<(bool productionExists, bool performerExists, bool roleExists)> CheckExists(int performerId, int productionId,
        int roleId);

    Task UpdateRange(List<Contribution> contributions);
    Task RemoveRange(List<Contribution> contributions);
    Task<List<Contribution>> CreateRange(List<Contribution> contributionsToCreate);
}

public class ContributionRepository : IContributionRepository
{
    private readonly TheatricalPlaysDbContext _context;
    private readonly ILogRepository _logRepository;
    private readonly ICaching _caching;

    public ContributionRepository(TheatricalPlaysDbContext context, ILogRepository logRepository, ICaching caching)
    {
        _context = context;
        _logRepository = logRepository;
        _caching = caching;
    }

    public async Task<Contribution> Create(Contribution contribution)
    {
        await _context.Contributions.AddAsync(contribution);
        await _context.SaveChangesAsync();

        var columns = new List<(string ColumnName, string Value)>
        {
            ("ID", contribution.Id.ToString()),
            ("PeopleID", contribution.PersonId.ToString()),
            ("ProductionID", contribution.ProductionId.ToString()),
            ("RoleID", contribution.RoleId.ToString())
        };

        if (contribution.SubRole != null)
        {
            columns.Add(("subRole", contribution.SubRole));
        }
        await _logRepository.UpdateLogs("insert", "contributions", columns);
        return contribution;
    }
    
    public async Task<List<Contribution>> Get()
    {
        var contributions = await _caching.GetOrSetAsync("allcontributions", async () =>
        {
            var contributions = await _context.Contributions.ToListAsync();
            return contributions;
        });

        return contributions;
    }

    public async Task<List<Contribution>> GetByPerformer(int personId)
    {
        var contributions = await _context.Contributions
            .Where(c => c.PersonId == personId)
            .ToListAsync();
        
        return contributions;
    }

    public async Task<List<Contribution>> GetByProduction(int productionId)
    {
        var contributions = await _context.Contributions
            .Where(c => c.ProductionId == productionId)
            .ToListAsync();

        return contributions;
    }

    public async Task<List<Contribution>> GetByRole(int roleId)
    {
        var contributions = await _context.Contributions
            .Where(c => c.RoleId == roleId)
            .ToListAsync();
        
        return contributions;
    }

    public async Task<List<Contribution>> GetSpecific(int personId, int productionId, int roleId)
    {
        var contributions = await _context.Contributions
            .Where(c => c.PersonId == personId && c.ProductionId == productionId && c.RoleId == roleId)
            .ToListAsync();
        
        return contributions;
    }

    public async Task<(bool productionExists, bool performerExists, bool roleExists)> CheckExists(int performerId, int productionId, int roleId)
    {
        bool productionExists = await _context.Productions.AnyAsync(p => p.Id == productionId);
        bool performerExists = await _context.Persons.AnyAsync(p => p.Id == performerId);
        bool roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);

        return (productionExists, performerExists, roleExists);
    }

    public async Task UpdateRange(List<Contribution> contributions)
    {
        _context.Contributions.UpdateRange(contributions);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRange(List<Contribution> contributions)
    {
        _context.Contributions.RemoveRange(contributions);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Contribution>> CreateRange(List<Contribution> contributionsToCreate)
    {
        await _context.Contributions.AddRangeAsync(contributionsToCreate);
        await _context.SaveChangesAsync();
        return contributionsToCreate;
    }
}

