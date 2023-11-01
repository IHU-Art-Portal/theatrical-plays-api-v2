using Theatrical.Data.Models;
using Theatrical.Dto.AccountRequestDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IAccountRequestService
{
    Task<List<AccountRequestDto>> GetAll();
}

public class AccountRequestService : IAccountRequestService
{
    private readonly IAccountRequestRepository _requestRepository;

    public AccountRequestService(IAccountRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<List<AccountRequestDto>> GetAll()
    {
        var requests = await _requestRepository.GetAll();
        
        var requestsDto = requests.Select(request => new AccountRequestDto
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

        return requestsDto;
    }
}

