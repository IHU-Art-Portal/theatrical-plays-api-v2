using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface ITransactionRepository
{
    Task<Transaction> PostTransaction(Transaction transaction);
    Task<List<Transaction>> GetTransactions(int userId);
    Task<Transaction?> GetTransaction(int transactionId);
    Task<List<User>> GetUsersWithVerifiedEmailNotPaid();
    Task PostTransactions(List<Transaction> transactions);
}

public class TransactionRepository : ITransactionRepository
{
    private readonly TheatricalPlaysDbContext _context;
    private readonly ILogRepository _logRepository;

    public TransactionRepository(TheatricalPlaysDbContext context, ILogRepository logRepository)
    {
        _context = context;
        _logRepository = logRepository;
    }

    public async Task<Transaction> PostTransaction(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        /*await _logRepository.UpdateLogs("insert", "transactions", new List<(string ColumnName, string Value)>
        {
            ("Id", transaction.Id.ToString()),
            ("UserId", transaction.UserId.ToString()),
            ("CreditAmount", transaction.CreditAmount.ToString(CultureInfo.CurrentCulture)),
            ("Reason", transaction.Reason)
        });*/
        return transaction;
    }
    
    public async Task<List<Transaction>> GetTransactions(int userId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .ToListAsync();

        return transactions;
    }

    public async Task<Transaction?> GetTransaction(int transactionId)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        return transaction;
    }

    public async Task<List<User>> GetUsersWithVerifiedEmailNotPaid()
    {
        return await _context.Users
            .Where(u => u.Enabled == true)
            .Where(u => u.UserTransactions.All(t => t.Reason != "Email Verification"))
            .ToListAsync();
    }

    public async Task PostTransactions(List<Transaction> transactions)
    {
        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();
    }
}