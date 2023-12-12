using System.Globalization;
using Theatrical.Data.Models;
using Theatrical.Dto.EventDtos;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IEventValidationService
{
    Task<ValidationReport> ValidateForCreate(CreateEventDto createEventDto);
    Task<(ValidationReport report, List<Event>? events)> FetchAndValidate();
    Task<(ValidationReport, Event?)> ValidateForFetch(int eventId);
    Task<(ValidationReport validation, List<Event>? events)> FetchEventsForPerson(int id);
    Task<(ValidationReport validation, List<Event>? events)> FetchEventsForProduction(int id);
    Task<(ValidationReport validation, User? user, Event? @event)> ValidateUserWithEventsForClaim(string email, int id);
    Task<(ValidationReport validation, UserEvent? userEvent)> ValidateForUpdate(List<UserEvent>? userEvents,
        UpdateEventDto1 updateEventDto1);
}

public class EventValidationService : IEventValidationService
{
    private readonly IEventRepository _repository;
    private readonly IProductionRepository _productionRepo;
    private readonly IVenueRepository _venueRepo;
    private readonly IUserEventRepository _userEventRepo;

    public EventValidationService(IEventRepository repository, IProductionRepository productionRepository, 
        IVenueRepository venueRepository, IUserEventRepository userEventRepository)
    {
        _repository = repository;
        _productionRepo = productionRepository;
        _venueRepo = venueRepository;
        _userEventRepo = userEventRepository;
    }

    public async Task<ValidationReport> ValidateForCreate(CreateEventDto createEventDto)
    {
        var report = new ValidationReport();
        var production = await _productionRepo.GetProduction(createEventDto.ProductionId);
        
        if (production is null)
        {
            report.Success = false;
            report.Message = $"Production with ID: {createEventDto.ProductionId} does not exist";
            return report;
        }

        var venue = await _venueRepo.Get(createEventDto.VenueId);

        if (venue is null)
        {
            report.Success = false;
            report.Message = $"Venue with Id: {createEventDto.VenueId} does not exist";
            return report;
        }

        report.Success = true;
        report.Message = "Success";
        return report;
    }

    public async Task<(ValidationReport report, List<Event>? events)> FetchAndValidate()
    {
        var events = await _repository.Get();
        var report = new ValidationReport();
        
        if (!events.Any())
        {
            report.Success = false;
            report.Message = "Not Found any Events";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Events Found";
        return (report, events);
    }

    public async Task<(ValidationReport, Event?)> ValidateForFetch(int eventId)
    {
        var eventFromDb = await _repository.GetEvent(eventId);
        var report = new ValidationReport();
        
        if (eventFromDb is null)
        {
            report.Success = false;
            report.Message = $"Event with id: {eventId} not found";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Success = true;
        report.Message = "Event found";

        return (report, eventFromDb);
    }

    public async Task<(ValidationReport validation, List<Event>? events)> FetchEventsForPerson(int id)
    {
        var events = await _repository.GetEventsForPerson(id);
        var report = new ValidationReport();

        if (!events.Any())
        {
            report.Success = false;
            report.Message = "Not Found events for this person";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Success = true;
        report.Message = "Found events";
        
        return (report, events);
    }

    public async Task<(ValidationReport validation, List<Event>? events)> FetchEventsForProduction(int id)
    {
        var events = await _repository.GetEventsForProduction(id);
        var report = new ValidationReport();
        
        if (!events.Any())
        {
            report.Success = false;
            report.Message = "Not Found events for this production";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        foreach (var @event in events)
        {
            Console.WriteLine(@event.Id);
        }

        report.Success = true;
        report.Message = "Found events";
        
        return (report, events);
    }

    public async Task<(ValidationReport validation, User? user, Event? @event)> ValidateUserWithEventsForClaim(string email, int id)
    {
        var userWithEvents = await _userEventRepo.GetUserWithEvents(email);
        var report = new ValidationReport();
        var @event = await _repository.GetEvent(id);

        if (@event is null)
        {
            report.Success = false;
            report.Message = "Event not found.";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null, null);
        }
        
        if (userWithEvents is null)
        {
            report.Success = false;
            report.Message = "User not found.";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null, null);
        }

        if (userWithEvents.UserEvents.Any(ue => ue.EventId == id))
        {
            report.Success = false;
            report.Message = "User has already claimed this event.";
            report.ErrorCode = ErrorCode.UserAlreadyClaimedEvent;
            return (report, null, null);
        }

        if (@event.IsClaimed)
        {
            report.Success = false;
            report.Message = "This event is already claimed by someone else.";
            report.ErrorCode = ErrorCode.UserAlreadyClaimedVenue;
            return (report, null, null);
        }
        
        report.Success = true;
        report.Message = "User can claim this venue";
        return (report, userWithEvents, @event);
        
    }

    public async Task<(ValidationReport validation, UserEvent? userEvent)> ValidateForUpdate(List<UserEvent>? userEvents, UpdateEventDto1 updateEventDto1)
    {
        var validation = new ValidationReport();

        if (string.IsNullOrEmpty(updateEventDto1.EventDate) || !DateTime.TryParseExact(updateEventDto1.EventDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            validation.Success = false;
            validation.Message = "Wrong date format. Correct format: dd-MM-yyyy HH:mm:ss. Date can't be left empty";
            validation.ErrorCode = ErrorCode.WrongDateFormat;
            return (validation, null);
        }
        
        if (userEvents != null && !userEvents.Any())
        {
            validation.Success = false;
            validation.Message = "You don't have any claimed events to edit.";
            validation.ErrorCode = ErrorCode.NotFound;
            return (validation, null);
            
        }

        var userEvent = userEvents?.FirstOrDefault(ue => ue.Event.Id == updateEventDto1.EventId);
        if (userEvent is null)
        {
            validation.Success = false;
            validation.Message = "You don't have permissions to edit this event or the event does not exist!";
            validation.ErrorCode = ErrorCode.BadRequest;
            return (validation, null);
        }

        if (updateEventDto1.VenueId != null)
        {
            var venue = await _venueRepo.Get((int)updateEventDto1.VenueId);
            if (venue is null)
            {
                validation.Success = false;
                validation.Message = "The venue you were trying to set for your event was not found.";
                validation.ErrorCode = ErrorCode.NotFound;
                return (validation, null);
            }
        }

        if (updateEventDto1.ProductionId != null)
        {
            var production = await _productionRepo.GetProduction((int)updateEventDto1.ProductionId);
            if (production is null)
            {
                validation.Success = false;
                validation.Message = "The production you were trying to set for your event was not found.";
                validation.ErrorCode = ErrorCode.NotFound;
                return (validation, null);
            }
        }

        validation.Success = true;
        return (validation, userEvent);
    }
}

