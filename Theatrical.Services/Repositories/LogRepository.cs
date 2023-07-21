using Microsoft.EntityFrameworkCore;
using Theatrical.Data.Context;
using Theatrical.Data.Models;

namespace Theatrical.Services.Repositories;

public interface ILogRepository
{
    Task<List<ChangeLog>> GetLogs();
    Task UpdateLogs(string eventType, string tableName, string value, string collumnName);
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
    /// Function to update the changeLog table.
    /// This function should be called after a delete/update/insert command.
    /// Use this on other repositories after executing one of the aforementioned events, in order to update the logs.
    /// Can be used more than once if that's necessary.
    /// </summary>
    /// <param name="eventType">string, insert or update or delete</param>
    /// <param name="tableName">string, name of the table</param>
    /// <param name="value">string, value that was effected</param>
    /// <param name="collumnName">string, collumn that was effected</param>
    public async Task UpdateLogs(string eventType, string tableName, string value, string collumnName)
    {
        var newLog = new ChangeLog
        {
            EventType = eventType,
            TableName = tableName,
            Value = value,
            CollumnName = collumnName
        };

        await _context.ChangeLogs.AddAsync(newLog);
        await _context.SaveChangesAsync();
    }
}