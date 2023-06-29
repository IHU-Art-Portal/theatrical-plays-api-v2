using Theatrical.Dto.Pagination;

namespace Theatrical.Services.Pagination;

public interface IPaginationService
{
    PaginationResult<TDto> GetPaginated<T, TDto>(int? page, int? size, IEnumerable<T> items,
        Func<IEnumerable<T>, IEnumerable<TDto>> mapper);
}

public class PaginationService : IPaginationService
{
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
                PageSize = null
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
            PageSize = pageCount
        };

        return response1;
    }

}