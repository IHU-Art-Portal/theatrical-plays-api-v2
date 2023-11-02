using Theatrical.Data.Models;

namespace Theatrical.Dto.AccountRequestDtos;

public class RequestActionDto
{
    public User ManagerUser { get; set; }
    public User Claimant { get; set; }
    public Person Person { get; set; }
    public AccountRequest AccountRequest { get; set; }
    public RequestManagerAction RequestManagerAction { get; set; }
}

public enum RequestManagerAction
{
    Reject = 0,
    Approve = 1,
}