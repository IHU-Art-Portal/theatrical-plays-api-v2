using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface ILogRepository
{
    Task<List<ChangeLog>> GetLogs();
    Task UpdateLogs(string eventType, string tableName, List<(string ColumnName, string Value)> columns);
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

    /// <summary>
    /// Updating logs method.
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="tableName"></param>
    /// <param name="columns"></param>
    public async Task UpdateLogs(string eventType, string tableName, List<(string ColumnName, string Value)> columns)
    {
        var newLog = new ChangeLog
        {
            EventType = eventType,
            TableName = tableName,
            Value = string.Join(", ", columns.Select(col => col.Value)),
            CollumnName = string.Join(", ", columns.Select(col => col.ColumnName))
        };

        await _context.ChangeLogs.AddAsync(newLog);
        await _context.SaveChangesAsync();
    }
}