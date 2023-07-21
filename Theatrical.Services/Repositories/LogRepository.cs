using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface ILogRepository
{
    Task<List<ChangeLog>> GetLogs();
}

public class LogRepository : ILogRepository
{
    private readonly TheatricalPlaysDbContext _context;

    public LogRepository(TheatricalPlaysDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChangeLog>> GetLogs()
    {
        var logs = await _context.ChangeLogs.ToListAsync();
        return logs;
    }
}