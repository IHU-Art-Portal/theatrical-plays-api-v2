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
        await _context.Persons.AddAsync(person);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Person>> Get()
    {
        var persons = await _context.Persons.ToListAsync();

        return persons;
    }
    
    public async Task<Person?> Get(int id)
    {
        var performer = await _context.Persons.FindAsync(id);

        return performer;
    }

    public async Task Delete(Person person)
    {
        _context.Persons.Remove(person);
        await _context.SaveChangesAsync();
    }
}