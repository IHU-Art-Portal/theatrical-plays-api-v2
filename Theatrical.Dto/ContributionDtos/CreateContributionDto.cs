namespace Theatrical.Dto.ContributionDtos;

public class CreateContributionDto
{
    public int PeopleId { get; set; }
    public int ProductionId { get; set; }
    public int RoleId { get; set; }
    public string? SubRole { get; set; }
}