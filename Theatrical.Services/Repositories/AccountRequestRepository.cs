using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface IAccountRequestRepository
{
    Task<AccountRequest> CreateRequest(AccountRequest accountRequest);
    Task<List<AccountRequest>> GetAll();
}

public class AccountRequestRepository : IAccountRequestRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public AccountRequestRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccountRequest>> GetAll()
    {
        return await _context.AccountRequests.ToListAsync();
    }

    public async Task<AccountRequest> CreateRequest(AccountRequest accountRequest)
    {
        await _context.AccountRequests.AddAsync(accountRequest);
        await _context.SaveChangesAsync();
        return accountRequest;
    }
}