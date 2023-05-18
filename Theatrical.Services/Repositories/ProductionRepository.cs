using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IProductionRepository
{
   Task Create(Production production);
   Task<List<Production>> Get();
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

    public async Task<List<Production>> Get()
    {
        return await _context.Productions.ToListAsync();
    }
}
