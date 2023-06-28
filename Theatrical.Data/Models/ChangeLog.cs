using System;
using System.Collections.Generic;

namespace Theatrical.Data.Models;

public class ChangeLog
{
    public int Id { get; set; }
    public string EventType { get; set; } = null!;
    public string TableName { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string CollumnName { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}