using Theatrical.Services.Security.Jwt;

namespace Theatrical.Services.Security.AuthorizationFilters;

public class ClaimsManagerAuthorizationFilter : AuthorizationFilterBase
{
    protected override string[] RequiredRole => new[] { "claims manager" };

    public ClaimsManagerAuthorizationFilter(ITokenService tokenService) : base(tokenService)
    {
    }

}