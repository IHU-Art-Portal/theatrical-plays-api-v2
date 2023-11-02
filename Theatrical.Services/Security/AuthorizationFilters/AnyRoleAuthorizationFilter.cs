using Theatrical.Services.Security.Jwt;

namespace Theatrical.Services.Security.AuthorizationFilters;

public class AnyRoleAuthorizationFilter : AuthorizationFilterBase
{
    protected override string[] RequiredRole => new[] { "admin", "user", "developer", "claims manager" };
    
    public AnyRoleAuthorizationFilter(ITokenService tokenService) : base(tokenService)
    {
    }


}