using Theatrical.Data.Models;
using Theatrical.Dto.ProductionDtos;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface IProductionService
{
    Task<ProductionDto> Create(CreateProductionDto createProductionDto);
    List<ProductionDto> ConvertToDto(List<Production> productions);
    Task<ProductionDto> Get(int productionId);
    Task Delete(Production production);
}
public class ProductionService : IProductionService
{
    private readonly IProductionRepository _repo;

    public ProductionService(IProductionRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProductionDto> Get(int productionId)
    {
        var production = await _repo.GetProduction(productionId);
        var productionDto = new ProductionDto
        {
            Id = production.Id,
            OrganizerId = production.OrganizerId,
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
            Created = DateTime.UtcNow
        };

        await _repo.Create(production);
        
        var productionDto = new ProductionDto
        {
            Id = production.Id,
            Description = production.Description,
            Duration = production.Duration,
            MediaUrl = production.MediaUrl,
            OrganizerId = production.OrganizerId,
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
                OrganizerId = production.OrganizerId,
                Producer = production.Producer,
                Title = production.Title,
                Url = production.Url
            };
            productionsDto.Add(productionDto);
        }

        return productionsDto;
    }
}

