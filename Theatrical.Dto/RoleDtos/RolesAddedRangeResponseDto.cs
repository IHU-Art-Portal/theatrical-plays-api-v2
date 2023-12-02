namespace Theatrical.Dto.RoleDtos;

public class RolesAddedRangeResponseDto
{
    public int CountAdded { get; set; }
    public int OutOf { get; set; }
    public List<RolesDtoShortened>? RolesAdded { get; set; }
    public List<RolesDtoShortened>? RolesExisted { get; set; }
}