using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IPersonRepository
{
    Task Create(Person person);
    Task<List<Person>> Get();
    Task Delete(Person person);
    Task<Person?> Get(int id);
}

public class PersonRepository : IPersonRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public PersonRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task Create(Person person)
    {
        await _context.persons.AddAsync(person);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Person>> Get()
    {
        var persons = await _context.persons.ToListAsync();

        return persons;
    }
    
    public async Task<Person?> Get(int id)
    {
        var performer = await _context.persons.FindAsync(id);

        return performer;
    }

    public async Task Delete(Person person)
    {
        _context.persons.Remove(person);
        await _context.SaveChangesAsync();
    }
}