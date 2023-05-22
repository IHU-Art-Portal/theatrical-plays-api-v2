using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IProductionRepository
{
   Task Create(Production production);
   Task<List<Production>>? Get();
   Task<Production?> GetProduction(int id);
   Task Delete(Production production);
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

    public async Task<Production?> GetProduction(int id)
    {
        var production = await _context.Productions.FindAsync(id);
        return production;
    }

    public async Task<List<Production>>? Get()
    {
        var productions = await _context.Productions.ToListAsync();
        return productions;
    }

    public async Task Delete(Production production)
    {
        _context.Productions.Remove(production);
        await _context.SaveChangesAsync();
    }
}
