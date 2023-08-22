using Theatrical.Services.Jwt;

namespace Theatrical.Api;

public class ClaimsManagerAuthorizationFilter : AuthorizationFilterBase
{
    protected override string[] RequiredRole => new[] { "claims manager" };

    public ClaimsManagerAuthorizationFilter(ITokenService tokenService) : base(tokenService)
    {
    }

}