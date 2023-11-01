using Theatrical.Data.Models;
using Theatrical.Dto.AccountRequestDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IAccountRequestService
{
    Task<List<AccountRequestDto>> GetAll(ConfirmationStatus? status);
}

public class AccountRequestService : IAccountRequestService
{
    private readonly IAccountRequestRepository _requestRepository;

    public AccountRequestService(IAccountRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    /// <summary>
    /// Returns every request if status is null
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public async Task<List<AccountRequestDto>> GetAll(ConfirmationStatus? status)
    {
        var requests = await _requestRepository.GetAll();
        
        var filteredRequests = requests
            .Where(request => status == null || request.ConfirmationStatus == status)
            .Select(request => new AccountRequestDto
            {
                Id = request.Id,
                UserId = request.UserId,
                PersonId = request.PersonId,
                IdentificationDocument = request.IdentificationDocument,
                CreatedAt = request.CreatedAt,
                AuthorizedBy = request.AuthorizedBy,
                ConfirmationStatus = request.ConfirmationStatus switch
                {
                    ConfirmationStatus.Active => "Active",
                    ConfirmationStatus.Approved => "Approved",
                    ConfirmationStatus.Rejected => "Rejected",
                    _ => "Invalid status"
                }
            }).ToList();
        
        return filteredRequests;
    }
}

