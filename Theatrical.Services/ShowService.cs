using System.Globalization;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ShowsLocal;
using Theatrical.Services.Pagination;

namespace Theatrical.Services;

public interface IShowService
{
    List<Show> ShowsFiltering(List<Show> shows, string? venueAddress, int? venueId, string? eventDate,
        string? organizerName, string? productionTitle);

    PaginationResult<Show> PaginateShows(int page, int size, List<Show> shows);
}

public class ShowService : IShowService
{
    private readonly IPaginationService _pagination;

    public ShowService(IPaginationService paginationService)
    {
        _pagination = paginationService;
    }
    public List<Show> ShowsFiltering(List<Show> shows, string? venueAddress, int? venueId, string? eventDate, 
        string? organizerName, string? productionTitle)
    {
        if (venueAddress is not null)
        {
            shows = shows
                .Where(s => s.VenueResponseDto != null && s.VenueResponseDto.Address.Contains(venueAddress, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (venueId is not null)
        {
            shows = shows
                .Where(s => s.VenueResponseDto != null && s.VenueResponseDto.Id == venueId)
                .ToList();
        }

        if (eventDate is not null)
        {
            if (DateTime.TryParseExact(eventDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                shows = shows
                    .Where(s => s.EventDate.HasValue && s.EventDate.Value.Date == parsedDate.Date)
                    .ToList();
            }
        }

        if (organizerName is not null)
        {
            shows = shows
                .Where(s => s.OrganizerShortenedDto.Name.Contains(organizerName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (productionTitle is not null)
        {
            shows = shows
                .Where(s => s.Production.Title.Contains(productionTitle, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return shows;
    }

    public PaginationResult<Show> PaginateShows(int page, int size, List<Show> shows)
    {
        return _pagination.GetPaginated(page, size, shows, items =>
        {
            return items.Select(show => new Show
            {
                OrganizerShortenedDto = show.OrganizerShortenedDto,
                VenueResponseDto = show.VenueResponseDto,
                Production = show.Production,
                Event = show.Event,
                EventDate = show.EventDate
            }).ToList();
        });
    }
}