using Theatrical.Services.Jwt;

namespace Theatrical.Api;

public class ClaimsManagerAndAdminAuthorizationFilter : AuthorizationFilterBase
{
    protected override string[] RequiredRole => new[] { "admin", "claims manager" };
    
    public ClaimsManagerAndAdminAuthorizationFilter(ITokenService tokenService) : base(tokenService)
    {
    }

    
}