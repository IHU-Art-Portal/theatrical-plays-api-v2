using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.enums;
using Theatrical.Data.Models;
using Theatrical.Dto.AccountRequestDtos;

namespace Theatrical.Services.Repositories;

public interface IAccountRequestRepository
{
    Task<AccountRequest> CreateRequest(AccountRequest accountRequest);
    Task<List<AccountRequest>> GetAll();
    Task ApproveRequest(RequestActionDto requestActionDto);
    Task RejectRequest(RequestActionDto requestActionDto);
    Task<AccountRequest?> Get(int requestId);
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

    public async Task ApproveRequest(RequestActionDto requestActionDto)
    {
        requestActionDto.AccountRequest.ConfirmationStatus = ConfirmationStatus.Approved;
        requestActionDto.AccountRequest.AuthorizedBy = requestActionDto.ManagerUser.Email;
        
        await _context.SaveChangesAsync();
    }

    public async Task RejectRequest(RequestActionDto requestActionDto)
    {
        requestActionDto.AccountRequest.ConfirmationStatus = ConfirmationStatus.Rejected;
        requestActionDto.AccountRequest.AuthorizedBy = requestActionDto.ManagerUser.Email;

        await _context.SaveChangesAsync();
    }

    public async Task<AccountRequest?> Get(int requestId)
    {
        var accountRequest = await _context.AccountRequests.FindAsync(requestId);
        return accountRequest;
    }
}