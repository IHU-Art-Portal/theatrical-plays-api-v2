using Theatrical.Data.Models;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface ITransactionValidationService
{
    Task<(ValidationReport, Transaction?)> ValidateForFetch(int transactionId);
    Task<(ValidationReport, List<Transaction>?)> ValidateUserTransactions(int userId);
}

public class TransactionValidationService : ITransactionValidationService
{
    private readonly ITransactionRepository _repo;
    private readonly IUserRepository _user;

    public TransactionValidationService(ITransactionRepository repository, IUserRepository userRepository)
    {
        _repo = repository;
        _user = userRepository;
    }

    public async Task<(ValidationReport, Transaction?)> ValidateForFetch(int transactionId)
    {
        var transaction = await _repo.GetTransaction(transactionId);
        var report = new ValidationReport();
        
        if (transaction is null)
        {
            report.Message = $"Transaction with id {transactionId} does not exist!";
            report.Success = false;
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Success = true;
        report.Message = "Transaction found";

        return (report, transaction);
    }

    public async Task<(ValidationReport, List<Transaction>?)> ValidateUserTransactions(int userId)
    {
        var user = await _user.Get(userId);
        var report = new ValidationReport();

        if (user is null)
        {
            report.Message = "User not found";
            report.ErrorCode = ErrorCode.NotFound;
            report.Success = false;
            return (report, null);
        }
        
        var transactions = await _repo.GetTransactions(userId);

        if (!transactions.Any())
        {
            report.Message = "This user does not have any transactions";
            report.Success = false;
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Message = "This user has transactions";
        report.Success = true;

        return (report, transactions);
    }
    
}

