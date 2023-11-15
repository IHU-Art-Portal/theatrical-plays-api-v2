using Theatrical.Data.Models;
using Theatrical.Dto.AccountRequestDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IAccountRequestService
{
    Task<List<AccountRequestDto>> GetAll(ConfirmationStatus? status);

    Task<ResponseAccountRequestDto> CreateRequest(Person person, User user,
        CreateAccountRequestDto createAccountRequestDto);

    Task RequestAction(RequestActionDto requestActionDto);
}

public class AccountRequestService : IAccountRequestService
{
    private readonly IAccountRequestRepository _requestRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IMinioService _minioService;
    private readonly IUserRepository _userRepository;
    private readonly IAssignedUserRepository _assignedUserRepository;

    public AccountRequestService(IAccountRequestRepository requestRepository, IPersonRepository personRepository, IMinioService minioService, IUserRepository userRepository,
        IAssignedUserRepository assignedUserRepository)
    {
        _requestRepository = requestRepository;
        _personRepository = personRepository;
        _minioService = minioService;
        _userRepository = userRepository;
        _assignedUserRepository = assignedUserRepository;
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
        var downloadUrl = await _minioService.PostPdf(createAccountRequestDto.IdentificationDocument, user.Id);
        
        var accountRequest = new AccountRequest
        {
            UserId = user.Id,
            PersonId = person.Id,
            ConfirmationStatus = ConfirmationStatus.Active,
            IdentificationDocument = downloadUrl
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

    //Accept or Reject a user request for a Person (pre-made account).
    //Run this after successfully passing validations.
    public async Task RequestAction(RequestActionDto requestActionDto)
    {
        
        if (requestActionDto.RequestManagerAction == RequestManagerAction.Approve)
        {
            await _requestRepository.ApproveRequest(requestActionDto);
            await _personRepository.ApproveRequest(requestActionDto);
            await _userRepository.OnRequestApproval(requestActionDto.Claimant, requestActionDto.Person);
            var assignedUser = new AssignedUser
            {
                UserId = requestActionDto.Claimant.Id,
                PersonId = requestActionDto.Person.Id,
                RequestId = requestActionDto.AccountRequest.Id
            };
            await _assignedUserRepository.AddAssignedPerson(assignedUser);
            return;
        }
    
        await _requestRepository.RejectRequest(requestActionDto);
        await _personRepository.RejectRequest(requestActionDto);
    }

    
}

