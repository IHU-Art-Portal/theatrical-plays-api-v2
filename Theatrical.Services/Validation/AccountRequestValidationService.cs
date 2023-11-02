using Theatrical.Data.Models;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IAccountRequestValidationService
{
    Task<(ValidationReport, AccountRequest?)> FetchAndValidate(int requestId);
}

public class AccountRequestRequestValidationService : IAccountRequestValidationService
{
    private readonly IAccountRequestRepository _accountRequestRepository;

    public AccountRequestRequestValidationService(IAccountRequestRepository accountRequestRepository)
    {
        _accountRequestRepository = accountRequestRepository;
    }

    public async Task<(ValidationReport, AccountRequest?)> FetchAndValidate(int requestId)
    {
        var report = new ValidationReport();
        var accountRequest = await _accountRequestRepository.Get(requestId);
        
        if (accountRequest is null)
        {
            report.Message = "Request not found";
            report.Success = false;
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Success = true;
        return (report, accountRequest);
    }
}