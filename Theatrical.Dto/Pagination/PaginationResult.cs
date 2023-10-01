namespace Theatrical.Dto.Pagination;

public class PaginationResult<T>
{
    public List<T>? Results { get; set; }
    public int? CurrentPage { get; set; }
    public int? TotalPages { get; set; }
}