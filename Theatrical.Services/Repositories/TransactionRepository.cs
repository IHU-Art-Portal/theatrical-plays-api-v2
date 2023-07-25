using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface ITransactionRepository
{
    Task PostTransaction(Transaction transaction);
}

public class TransactionRepository : ITransactionRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public TransactionRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task PostTransaction(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }
}