using Theatrical.Services.Jwt;

namespace Theatrical.Api;

public class AnyRoleAuthorizationFilter : AuthorizationFilterBase
{
    protected override string[] RequiredRole => new[] { "admin", "user", "developer" };
    
    public AnyRoleAuthorizationFilter(ITokenService tokenService) : base(tokenService)
    {
    }


}