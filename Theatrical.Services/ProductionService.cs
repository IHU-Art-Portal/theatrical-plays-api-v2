using Theatrical.Data.Models;
using Theatrical.Dto.Pagination;
using Theatrical.Dto.ProductionDtos;
using Theatrical.Services.Pagination;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IProductionService
{
    Task<ProductionDto> Create(CreateProductionDto createProductionDto);
    List<ProductionDto> ConvertToDto(List<Production> productions);
    ProductionDto ToDto(Production production);
    Task Delete(Production production);
    PaginationResult<ProductionDto> Paginate(int? page, int? size, List<ProductionDto> productionsDto);
}
public class ProductionService : IProductionService
{
    private readonly IProductionRepository _repo;
    private readonly IPaginationService _pagination;

    public ProductionService(IProductionRepository repo, IPaginationService paginationService)
    {
        _repo = repo;
        _pagination = paginationService;
    }

    public ProductionDto ToDto(Production production)
    {
        var productionDto = new ProductionDto
        {
            Id = production.Id,
            OrganizerId = (int)production.OrganizerId,
            Title = production.Title,
            Description = production.Description,
            Url = production.Url,
            Producer = production.Producer,
            MediaUrl = production.MediaUrl,
            Duration = production.Duration
        };

        return productionDto;
    }
    
    public async Task<ProductionDto> Create(CreateProductionDto createProductionDto)
    {
        var production = new Production
        {
            OrganizerId = createProductionDto.OrganizerId,
            Title = createProductionDto.Title,
            Description = createProductionDto.Description,
            Url = createProductionDto.Url,
            Producer = createProductionDto.Producer,
            MediaUrl = createProductionDto.MediaUrl,
            Duration = createProductionDto.Duration,
            Timestamp = DateTime.UtcNow
        };

        await _repo.Create(production);
        
        var productionDto = new ProductionDto
        {
            Id = production.Id,
            Description = production.Description,
            Duration = production.Duration,
            MediaUrl = production.MediaUrl,
            OrganizerId = (int)production.OrganizerId,
            Producer = production.Producer,
            Title = production.Title,
            Url = production.Url
        };
        
        return productionDto;
    }

    public async Task Delete(Production production)
    {
        await _repo.Delete(production);
    }
    
    public List<ProductionDto> ConvertToDto(List<Production> productions)
    {
        var productionsDto = new List<ProductionDto>();

        foreach (var production in productions)
        {
            var productionDto = new ProductionDto
            {
                Id = production.Id,
                Description = production.Description,
                Duration = production.Duration,
                MediaUrl = production.MediaUrl,
                OrganizerId = (int)production.OrganizerId,
                Producer = production.Producer,
                Title = production.Title,
                Url = production.Url
            };
            productionsDto.Add(productionDto);
        }

        return productionsDto;
    }

    public PaginationResult<ProductionDto> Paginate(int? page, int? size, List<ProductionDto> productionsDto)
    {
        var paginationResult = _pagination.GetPaginated(page, size, productionsDto, items =>
        {
            return items.Select(prod => new ProductionDto
            {
                Id = prod.Id,
                OrganizerId = prod.OrganizerId,
                Title = prod.Title,
                Description = prod.Description,
                Url = prod.Url,
                Producer = prod.Producer,
                MediaUrl = prod.MediaUrl,
                Duration = prod.Duration
            });
        });

        return paginationResult;
    }
}

