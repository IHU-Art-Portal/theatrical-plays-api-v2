namespace Theatrical.Dto.LogDtos;

public class LogDto
{
    public int Id { get; set; }
    public string EventType { get; set; } 
    public string TableName { get; set; }
    public string Value { get; set; }
    public string CollumnName { get; set; }
    public DateTime Timestamp { get; set; }
}