using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IPerformerRepository
{
    Task Create(Performer performer);
    Task<List<Performer>> Get();
}

public class PerformerRepository : IPerformerRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public PerformerRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task Create(Performer performer)
    {
        await _context.Performers.AddAsync(performer);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Performer>> Get()
    {
        var performers = await _context.Performers.ToListAsync();

        return performers;
    }
}