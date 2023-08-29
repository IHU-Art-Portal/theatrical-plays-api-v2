using Theatrical.Data.Models;

namespace Theatrical.Services.Curators.Responses;

public class CurateResponseRoles
{
    public List<Role> Roles {get; set; }
    public int CorrectedObjects { get; set; }
    public int OutOf { get; set; }
}