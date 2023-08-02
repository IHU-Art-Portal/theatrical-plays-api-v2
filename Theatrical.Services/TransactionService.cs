using Theatrical.Data.Models;
using Theatrical.Dto.TransactionDtos;

namespace Theatrical.Services;

public interface ITransactionService
{
    TransactionDtoFetch TransactionToDto(Transaction transaction);
    List<TransactionDtoFetch> TransactionListToDto(List<Transaction> transactions);
}

public class TransactionService : ITransactionService
{
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

