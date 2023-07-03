using Theatrical.Data.Models;

namespace Theatrical.Dto.PersonDtos;

public class PersonProductionsRoleInfo
{
    public Production Production { get; set; }
    public Role Role { get; set; }
}