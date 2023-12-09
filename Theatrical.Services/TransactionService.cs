using Theatrical.Data.Models;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Dto.TransactionDtos.PurchaseDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface ITransactionService
{
    TransactionDtoFetch TransactionToDto(Transaction transaction);
    TransactionResponseDto TransactionToResponseDto(Transaction transcation);
    List<TransactionDtoFetch> TransactionListToDto(List<Transaction> transactions);
    Task<Transaction> PostTransaction(User user, long? amountTotalInEuros);
    Task VerifiedEmailCredits(User user);
    Task VerifiedEmailCredits(List<User> usersNotPaid);
    Task<List<User>> GetUsersWithVerifiedEmailNotPaid();
}

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;

    public TransactionService(ITransactionRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Transaction> PostTransaction(User user, long? amountTotalInEuros)
    {
        var transaction = new Transaction
        {
            UserId = user.Id,
            CreditAmount = Convert.ToDecimal(amountTotalInEuros),
            Reason = "Credit Purchase",
        };
        
        var newTransaction = await _repository.PostTransaction(transaction);
        return newTransaction;
    }

    public async Task VerifiedEmailCredits(User user)
    {
        var transaction = new Transaction
        {
            UserId = user.Id,
            CreditAmount = 1.01m,
            Reason = "Email Verification",
        };
        
        await _repository.PostTransaction(transaction);
    }

    public async Task VerifiedEmailCredits(List<User> usersNotPaid)
    {
        var transactions = usersNotPaid.Select(u => new Transaction
        {
            UserId = u.Id,
            CreditAmount = 1.01m,
            Reason = "Email Verification",
        }).ToList();

        await _repository.PostTransactions(transactions);
    }

    public async Task<List<User>> GetUsersWithVerifiedEmailNotPaid()
    {
        return await _repository.GetUsersWithVerifiedEmailNotPaid();
    }

    public TransactionDtoFetch TransactionToDto(Transaction transaction)
    {

        var transactionDtoFetch = new TransactionDtoFetch
        {
            CreditAmount = transaction.CreditAmount,
            Reason = transaction.Reason,
            UserId = transaction.UserId,
            DateCreated = transaction.DateCreated,
        };

        return transactionDtoFetch;
    }

    public TransactionResponseDto TransactionToResponseDto(Transaction transaction)
    {
        var transactionResponseDto = new TransactionResponseDto
        {
            CreditAmount = transaction.CreditAmount,
            Reason = transaction.Reason,
            UserId = transaction.UserId,
            DateCreated = transaction.DateCreated,
            DatabaseTransactionId = transaction.Id
        };

        return transactionResponseDto;
    }

    public List<TransactionDtoFetch> TransactionListToDto(List<Transaction> transactions)
    {
        List<TransactionDtoFetch> transactionDtoFetches = 
            transactions.Select(transaction => new TransactionDtoFetch 
                { CreditAmount = transaction.CreditAmount, 
                    Reason = transaction.Reason, 
                    UserId = transaction.UserId, 
                    DateCreated = transaction.DateCreated,
                }).ToList();
        
        return transactionDtoFetches;
    }
}

