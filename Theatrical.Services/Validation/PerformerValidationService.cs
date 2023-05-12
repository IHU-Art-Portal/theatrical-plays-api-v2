using Theatrical.Data.Context;
using Theatrical.Data.Models;
using Theatrical.Dto.PerformerDtos;

namespace Theatrical.Services.Validation;

public interface IPerformerValidationService
{
    Task<(ValidationReport report, Performer? performer)> ValidateAndFetch(int performerId);
    Task<(ValidationReport report, Performer? performer)> ValidateForDelete(int performerId);
}

public class PerformerValidationService : IPerformerValidationService
{
    private readonly TheatricalPlaysDbContext _context;

    public PerformerValidationService(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task<(ValidationReport report, Performer? performer)> ValidateAndFetch(int performerId)
    {
        var performer = await _context.Performers.FindAsync(performerId);
        var report = new ValidationReport();

        if (performer is null)
        {
            report.Success = false;
            report.Message = "Performer not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Performer exists";
        return (report, performer);
    }

    public async Task<(ValidationReport report, Performer? performer)> ValidateForDelete(int performerId)
    {
        var performer = await _context.Performers.FindAsync(performerId);
        var report = new ValidationReport();
        
        if (performer is null)
        {
            report.Success = false;
            report.Message = "Performer not found";
            return (report, null);
        }

        report.Success = true;
        report.Message = "Performer exists and marked for deletion";
        return (report, performer);
    }
}