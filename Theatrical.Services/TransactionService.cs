using Theatrical.Data.Models;
using Theatrical.Dto.TransactionDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface ITransactionService
{
    TransactionDtoFetch TransactionToDto(Transaction transaction);
    List<TransactionDtoFetch> TransactionListToDto(List<Transaction> transactions);
    Task<Transaction> PostTransaction(TransactionDto transactionDto);
}

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;

    public TransactionService(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Transaction> PostTransaction(TransactionDto transactionDto)
    {
        var transaction = new Transaction
        {
            UserId = transactionDto.UserId,
            CreditAmount = transactionDto.CreditAmount,
            Reason = transactionDto.Reason
        };
        
        var newTransaction = await _repository.PostTransaction(transaction);
        return newTransaction;
    }
    
    public TransactionDtoFetch TransactionToDto(Transaction transaction)
    {

        var transactionDto = new TransactionDtoFetch
        {
            CreditAmount = transaction.CreditAmount,
            Reason = transaction.Reason,
            UserId = transaction.UserId,
            DateCreated = transaction.DateCreated
        };

        return transactionDto;
    }

    public List<TransactionDtoFetch> TransactionListToDto(List<Transaction> transactions)
    {
        List<TransactionDtoFetch> transactionDtoFetches = 
            transactions.Select(transaction => new TransactionDtoFetch 
                { CreditAmount = transaction.CreditAmount, Reason = transaction.Reason, 
                    UserId = transaction.UserId, DateCreated = transaction.DateCreated 
                }).ToList();
        
        return transactionDtoFetches;
    }
}

