using Theatrical.Data.Models;

namespace Theatrical.Dto.RoleDtos;

public class CuratorRoleResponse
{
    public List<Role> roles { get; set; }
    public int CorrectedRoles { get; set; }
    public int TotalRoles { get; set; }
}