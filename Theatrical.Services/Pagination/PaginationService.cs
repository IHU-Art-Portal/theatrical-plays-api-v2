using Theatrical.Dto.Pagination;

namespace Theatrical.Services.Pagination;

public interface IPaginationService
{
    PaginationResult<TDto> GetPaginated<T, TDto>(int? page, int? size, IEnumerable<T> items,
        Func<IEnumerable<T>, IEnumerable<TDto>> mapper);
}

public class PaginationService : IPaginationService
{
    /// <summary>
    /// /// Pagination Behavior:
    ///           only page: returns the specified page, with 10 results per page,
    ///           only size: returns always the 1st page, with specified sized results.
    /// </summary>
    /// <param name="page">integer</param>
    /// <param name="size">integer</param>
    /// <param name="items">the list of X that you want to paginate</param>
    /// <param name="mapper">map your items that you want to include</param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDto"></typeparam>
    /// <returns>A PaginationResult with the desired results.</returns>
    public PaginationResult<TDto> GetPaginated<T, TDto>(int? page, int? size, IEnumerable<T> items, Func<IEnumerable<T>, IEnumerable<TDto>> mapper)
    {
        List<TDto> dtos = new();
    
        if (page is null && size is null)
        {
            dtos.AddRange(mapper(items));

            var response = new PaginationResult<TDto>
            {
                Results = dtos,
                CurrentPage = null,
                TotalPages = null
            };

            return response;
        }

        size ??= 10;
        page ??= 1;

        var pageResults = (float)size;
        var pageCount = (int)Math.Ceiling(items.Count() / pageResults);

        var itemsPaged = items
            .Skip((page.Value - 1) * (int)pageResults)
            .Take((int)pageResults);

        dtos.AddRange(mapper(itemsPaged));

        var response1 = new PaginationResult<TDto>
        {
            Results = dtos,
            CurrentPage = page,
            TotalPages = pageCount
        };

        return response1;
    }

}