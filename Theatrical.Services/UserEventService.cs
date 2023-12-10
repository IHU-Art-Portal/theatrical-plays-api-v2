using Theatrical.Data.Models;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IUserEventService
{
    Task CreateUserEvent(User user, Event @event);
}

public class UserEventService : IUserEventService
{
    private readonly IUserEventRepository _repository;

    public UserEventService(IUserEventRepository userEventRepository)
    {
        _repository = userEventRepository;
    }

    public async Task CreateUserEvent(User user, Event @event)
    {
        var userEvent = new UserEvent
        {
            UserId = user.Id,
            EventId = @event.Id
        };

        await _repository.Claim(@event);
        await _repository.Create(userEvent);
    }
}

