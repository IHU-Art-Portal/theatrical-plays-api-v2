using Theatrical.Services.Jwt;

namespace Theatrical.Api;

public class UserAuthorizationFilter : AuthorizationFilterBase
{
    protected override string[] RequiredRole => new[] {"user"};

    public UserAuthorizationFilter(ITokenService tokenService) : base(tokenService)
    {
    }
}