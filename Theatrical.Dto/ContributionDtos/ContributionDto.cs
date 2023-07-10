namespace Theatrical.Dto.ContributionDtos;

public class ContributionDto
{
    public int Id { get; set; }
    public int PeopleId { get; set; }
    public int ProductionId { get; set; }
    public int RoleId { get; set; }
    public string? SubRole { get; set; }
    public int SystemId { get; set; }
    public DateTime Timestamp { get; set; }
}