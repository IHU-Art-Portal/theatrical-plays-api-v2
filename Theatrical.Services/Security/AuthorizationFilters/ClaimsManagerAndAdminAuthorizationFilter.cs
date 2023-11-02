using Theatrical.Services.Security.Jwt;

namespace Theatrical.Services.Security.AuthorizationFilters;

public class ClaimsManagerAndAdminAuthorizationFilter : AuthorizationFilterBase
{
    protected override string[] RequiredRole => new[] { "admin", "claims manager" };
    
    public ClaimsManagerAndAdminAuthorizationFilter(ITokenService tokenService) : base(tokenService)
    {
    }

    
}