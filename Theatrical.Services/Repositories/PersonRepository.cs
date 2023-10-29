using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Dto.PersonDtos;

namespace Theatrical.Services.Repositories;

public interface IPersonRepository
{
    Task<Person> Create(Person person);
    Task<List<Person>> Get();
    Task Delete(Person person);
    Task<Person?> Get(int id);
    Task<List<Person>?> GetByRole(string role);
    Task<List<Person>?> GetByLetter(string initials);
    Task<Person?> GetByName(string name);
    Task<List<PersonProductionsRoleInfo>?> GetProductionsOfPerson(int personId);
    Task<List<Image>?> GetPersonsImages(int personId);
    Task UpdateRange(List<Person> people);
}

public class PersonRepository : IPersonRepository
{
    private readonly TheatricalPlaysDbContext _context;
    private readonly IMemoryCache _memoryCache;

    public PersonRepository(TheatricalPlaysDbContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    public async Task<Person> Create(Person person)
    {
        await _context.Persons.AddAsync(person);
        await _context.SaveChangesAsync();
        return person;
    }
    
    public async Task<List<Person>> Get()
    {
        if (!_memoryCache.TryGetValue("allpeople", out List<Person> people))
        {
            people = await _context.Persons.ToListAsync();
            _memoryCache.Set("allpeople", people);
            return people;
        }

        return people;
    }
    
    public async Task<Person?> Get(int id)
    {
        if (!_memoryCache.TryGetValue($"person_{id}", out Person? person))
        {
            person = await _context.Persons.FindAsync(id);
            
            if (person != null)
            {
                _memoryCache.Set($"person_{id}", person, TimeSpan.FromMinutes(15)); //Cache for 15 minutes
            }
        }

        return person;
    }

    public async Task<List<Person>?> GetByRole(string role)
    {
        if (!_memoryCache.TryGetValue($"persons_with_role_{role}", out List<Person> personsWithRole))
        {
            personsWithRole = await _context.Persons.Where(p => p.Contributions
                    .Any(c => c.Role.Role1 == role))
                .ToListAsync();

            if (personsWithRole.Count > 0)
            {
                _memoryCache.Set($"persons_with_role_{role}", personsWithRole, TimeSpan.FromMinutes(15));
            }
        }
        
        return personsWithRole;
    }

    public async Task<List<Person>?> GetByLetter(string initials)
    {
        if (!_memoryCache.TryGetValue($"persons_by_initials_{initials}", out List<Person> persons))
        {
            persons = await _context.Persons.Where(p => p.Fullname.StartsWith(initials)).ToListAsync();

            if (persons.Count > 0)
            {
                _memoryCache.Set($"persons_by_initials_{initials}", persons, TimeSpan.FromMinutes(15));
            }
        }
        

        return persons;
    }

    public async Task<Person?> GetByName(string name)
    {
        if (!_memoryCache.TryGetValue($"person_by_name_{name}", out Person? person))
        {
            person = await _context.Persons.FirstOrDefaultAsync(p => p.Fullname == name);

            if (person != null)
            {
                _memoryCache.Set($"person_by_name_{name}", person, TimeSpan.FromMinutes(15));
            }
        }

        return person;
    }

    public async Task Delete(Person person)
    {
        _context.Persons.Remove(person);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PersonProductionsRoleInfo>?> GetProductionsOfPerson(int personId)
    {
        if (!_memoryCache.TryGetValue($"productions_person_{personId}", out List<PersonProductionsRoleInfo> personProductions))
        {
            personProductions = await _context.Contributions
                .Where(c => c.PeopleId == personId)
                .Include(c => c.Role)
                .Select(c => new PersonProductionsRoleInfo
                {
                    Production = c.Production,
                    Role = c.Role
                })
                .ToListAsync();

            if (personProductions.Count > 0)
            {
                _memoryCache.Set($"productions_person_{personId}", personProductions, TimeSpan.FromMinutes(15));
            }
        }
        
        
        return personProductions;
    }

    public async Task<List<Image>?> GetPersonsImages(int personId)
    {
        if (!_memoryCache.TryGetValue($"persons_images_{personId}", out List<Image> images))
        {
            var person = await _context.Persons
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == personId);

            if (person != null)
            {
                images = person.Images.ToList();
                _memoryCache.Set($"persons_images_{personId}", images, TimeSpan.FromMinutes(15));
            }
        }

        return images;
    }

    public async Task UpdateRange(List<Person> people)
    {
        _context.Persons.UpdateRange(people);
        await _context.SaveChangesAsync();
    }
}