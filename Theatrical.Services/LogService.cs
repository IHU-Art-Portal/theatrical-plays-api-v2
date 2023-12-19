using Theatrical.Dto.LogDtos;
using Theatrical.Dto.Pagination;
using Theatrical.Services.Pagination;
using Theatrical.Services.Repositories;

namespace Theatrical.Services;

public interface ILogService
{
    Task<List<LogDto>> GetLogs();
    PaginationResult<LogDto> Paginate(int? page, int? size, List<LogDto> logDtos);
}

public class LogService : ILogService
{
    private readonly ILogRepository _repository;
    private readonly IPaginationService _pagination;

    public LogService(ILogRepository repository, IPaginationService paginationService)
    {
        _repository = repository;
        _pagination = paginationService;
    }

    public async Task<List<LogDto>> GetLogs()
    {
        var logs = await _repository.GetLogs();
        var logsDto = logs.Select(l => new LogDto
        {
            CollumnName = l.CollumnName,
            EventType = l.EventType,
            Id = l.Id,
            TableName = l.TableName,
            Timestamp = l.Timestamp,
            Value = l.Value
        }).ToList();
        
        return logsDto;
    }
    
    public PaginationResult<LogDto> Paginate(int? page, int? size, List<LogDto> logDtos)
    {
        var result = _pagination.GetPaginated(page, size, logDtos, items =>
        {
            return items.Select(log => new LogDto
            {
                CollumnName = log.CollumnName,
                EventType = log.EventType,
                Id = log.Id,
                TableName = log.TableName,
                Timestamp = log.Timestamp,
                Value = log.Value
            });
        });
        
        return result;
    }
}