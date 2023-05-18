using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IProductionRepository
{
    
}

public class ProductionRepository : IProductionRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public ProductionRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task Create(Production production)
    {
        await _context.Productions.AddAsync(production);
        await _context.SaveChangesAsync();
    }
}
