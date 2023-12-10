using Theatrical.Data.Models;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Dto.VenueDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services.Validation;

public interface IVenueValidationService
{
    Task<(ValidationReport report, List<Venue>? venue)> ValidateAndFetch();
    Task<(ValidationReport report, Venue? venue)> ValidateAndFetch(int venueId);
    Task<(ValidationReport report, Venue? venue)> ValidateForDelete(int venueId);
    Task<ValidationReport> ValidateForUpdate(VenueUpdateDto venueDto);
    Task<(ValidationReport, List<Production>?)> ValidateAndFetchVenueProductions(int venueId);
    Task<(ValidationReport, Venue? venue)> ValidateAndFetch(string venueTitle);
    Task<(ValidationReport validation, User? user, Venue? venue)> ValidateUserWithVenuesForClaim(string email, int id);
}

public class VenueValidationService :  IVenueValidationService
{
    private readonly IVenueRepository _repository;
    private readonly IUserVenueRepository _uvRepository;

    public VenueValidationService(IVenueRepository repository, IUserVenueRepository userVenueRepository)
    {
        _repository = repository;
        _uvRepository = userVenueRepository;
    }

    public async Task<(ValidationReport report, List<Venue>? venue)> ValidateAndFetch()
    {
        var report = new ValidationReport();
        var venues = await _repository.Get();

        if (!venues.Any())
        {
            report.Success = false;
            report.Message = "Not any Venues exist";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Venues found";
        return (report, venues);
    }
    
    public async Task<(ValidationReport report, Venue? venue)> ValidateAndFetch(int venueId)
    {
        var report = new ValidationReport();
        var venue = await _repository.Get(venueId);

        if (venue is null)
        {
            report.Success = false;
            report.Message = "Venue not found";
            return (report, null);
        }
        
        report.Success = true;
        report.Message = "Venue found";
        return (report, venue);
    }

    public async Task<(ValidationReport report, Venue? venue)> ValidateForDelete(int venueId)
    {
        var report = new ValidationReport();
        var venue = await _repository.Get(venueId);

        if (venue is null)
        {
            report.Success = false;
            report.Message = "Venue not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Venue found and marked for delete";
        return (report, venue);
    }

    public async Task<ValidationReport> ValidateForUpdate(VenueUpdateDto venueDto)
    {
        var report = new ValidationReport();
        var venue = await _repository.Get(venueDto.Id);

        if (venue is null)
        {
            report.Success = false;
            report.Message = "Venue not found";
            return report;
        }
        
        report.Success = true;
        report.Message = "Venue can be updated";
        return report;
    }

    public async Task<(ValidationReport, List<Production>?)> ValidateAndFetchVenueProductions(int venueId)
    {
        var productions = await _repository.GetVenueProductions(venueId);
        var venue = await _repository.Get(venueId);
        var report = new ValidationReport();

        if (venue is null)
        {
            report.Success = false;
            report.Message = $"Venue with id {venueId} not found";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null);
        }

        report.Success = true;
        return (report, productions);
    }

    public async Task<(ValidationReport, Venue? venue)> ValidateAndFetch(string venueTitle)
    {
        var venue = await _repository.GetVenueByTitle(venueTitle.Trim());

        if (venue is not null)
        {
            return (new ValidationReport
            {
                Success = false,
                Message = "Venue already exists and cannot be created again.",
                ErrorCode = ErrorCode.AlreadyExists
            }, venue);
        }

        return (new ValidationReport
        {
            Success = true
        }, null);
    }

    public async Task<(ValidationReport validation, User? user, Venue? venue)> ValidateUserWithVenuesForClaim(string email, int id)
    {
        var user = await _uvRepository.GetUserWithVenues(email);
        var report = new ValidationReport();
        var venue = await _repository.Get(id);

        if (venue is null)
        {
            report.Success = false;
            report.Message = "Venue not found.";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null, null);
        }

        if (user is null)
        {
            report.Success = false;
            report.Message = "User not found.";
            report.ErrorCode = ErrorCode.NotFound;
            return (report, null, null);
        }

        if (user.UserVenues.Any(uv => uv.VenueId == id))
        {
            report.Success = false;
            report.Message = "User has already claimed this venue.";
            report.ErrorCode = ErrorCode.UserAlreadyClaimedVenue;
            return (report, null, null);
        }

        if (venue.isClaimed)
        {
            report.Success = false;
            report.Message = "This venue is already claimed by someone else.";
            report.ErrorCode = ErrorCode.UserAlreadyClaimedVenue;
            return (report, null, null);
        }

        report.Success = true;
        report.Message = "User can claim this venue";
        return (report, user, venue);
    }
}