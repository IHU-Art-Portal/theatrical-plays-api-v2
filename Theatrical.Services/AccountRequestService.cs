using Theatrical.Data.Models;
using Theatrical.Dto.AccountRequestDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IAccountRequestService
{
    Task<List<AccountRequestDto>> GetAll(ConfirmationStatus? status);

    Task<ResponseAccountRequestDto> CreateRequest(Person person, User user,
        CreateAccountRequestDto createAccountRequestDto);
}

public class AccountRequestService : IAccountRequestService
{
    private readonly IAccountRequestRepository _requestRepository;
    private readonly IPersonRepository _personRepository;

    public AccountRequestService(IAccountRequestRepository requestRepository, IPersonRepository personRepository)
    {
        _requestRepository = requestRepository;
        _personRepository = personRepository;
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
    
    public async Task<ResponseAccountRequestDto> CreateRequest(Person person, User user, CreateAccountRequestDto createAccountRequestDto)
    {
        var accountRequest = new AccountRequest
        {
            UserId = user.Id,
            PersonId = person.Id,
            ConfirmationStatus = ConfirmationStatus.Active,
            IdentificationDocument = createAccountRequestDto.IdentificationDocument
        };

        await _personRepository.CreateRequest(person);                                //changes the claiming status to 1 (Unavailable)
        var requestResponse = await _requestRepository.CreateRequest(accountRequest); //Creates an entry in account requests table

        var accountRequestDto = new ResponseAccountRequestDto
        {
            Status = "Active",
            Id = requestResponse.Id,
            PersonId = requestResponse.PersonId,
            UserId = requestResponse.UserId
        };

        return accountRequestDto;
    }
}

