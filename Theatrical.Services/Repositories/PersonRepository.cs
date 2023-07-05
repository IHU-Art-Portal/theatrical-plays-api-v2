using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Dto.PersonDtos;
using Theatrical.Services.PerformersService;

namespace Theatrical.Services.Repositories;

public interface IPersonRepository
{
    Task Create(Person person);
    Task<List<Person>> Get();
    Task Delete(Person person);
    Task<Person?> Get(int id);
    Task<List<Person>?> GetByRole(string role);
    Task<List<Person>?> GetByLetter(string initials);
    Task<Person?> GetByName(string name);
    Task<List<PersonProductionsRoleInfo>?> GetProductionsOfPerson(int personId);
    Task<List<Image>?> GetPersonsImages(int personId);
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
        var person = await _context.Persons.FindAsync(id);

        return person;
    }

    public async Task<List<Person>?> GetByRole(string role)
    {
        var personsWithRole = await _context.Persons.Where(p => p.Contributions
                .Any(c => c.Role.Role1 == role))
            .ToListAsync();

        return personsWithRole;
    }

    public async Task<List<Person>?> GetByLetter(string initials)
    {
        var persons = await _context.Persons.Where(p => p.Fullname.StartsWith(initials)).ToListAsync();

        return persons;
    }

    public async Task<Person?> GetByName(string name)
    {
        return await _context.Persons.FirstOrDefaultAsync(p => p.Fullname == name);
    }

    public async Task Delete(Person person)
    {
        _context.Persons.Remove(person);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PersonProductionsRoleInfo>?> GetProductionsOfPerson(int personId)
    {
        var personProductions = await _context.Contributions
            .Where(c => c.PeopleId == personId)
            .Include(c => c.Role)
            .Select(c => new PersonProductionsRoleInfo
            {
                Production = c.Production,
                Role = c.Role
            })
            .ToListAsync();
    
        return personProductions;
    }

    public async Task<List<Image>?> GetPersonsImages(int personId)
    {
        var person = await _context.Persons.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == personId);

        return person.Images;
    }
}