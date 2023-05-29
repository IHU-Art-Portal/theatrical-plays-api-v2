using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IContributionRepository
{
    Task<List<Contribution>> Get();
    Task Create(Contribution contribution);
    Task<List<Contribution>> GetSpecific(int performerId, int productionId, int roleId);
    Task<List<Contribution>> GetByRole(int roleId);
    Task<List<Contribution>> GetByProduction(int productionId);
    Task<List<Contribution>> GetByPerformer(int performerId);

    Task<(bool productionExists, bool performerExists, bool roleExists)> CheckExists(int performerId, int productionId,
        int roleId);
}

public class ContributionRepository : IContributionRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public ContributionRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task Create(Contribution contribution)
    {
        await _context.Contributions.AddAsync(contribution);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Contribution>> Get()
    {
        var contributions = await _context.Contributions.ToListAsync();
        return contributions;
    }

    public async Task<List<Contribution>> GetByPerformer(int performerId)
    {
        var contributions = await _context.Contributions
            .Where(c => c.PerformerId == performerId)
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

    public async Task<List<Contribution>> GetSpecific(int performerId, int productionId, int roleId)
    {
        var contributions = await _context.Contributions
            .Where(c => c.PerformerId == performerId && c.ProductionId == productionId && c.RoleId == roleId)
            .ToListAsync();
        
        return contributions;
    }

    public async Task<(bool productionExists, bool performerExists, bool roleExists)> CheckExists(int performerId, int productionId, int roleId)
    {
        bool productionExists = await _context.Productions.AnyAsync(p => p.Id == productionId);
        bool performerExists = await _context.Performers.AnyAsync(p => p.Id == performerId);
        bool roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);

        return (productionExists, performerExists, roleExists);
    }
}

