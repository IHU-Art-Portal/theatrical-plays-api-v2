using Theatrical.Services.Jwt;

namespace Theatrical.Api;

public class AdminAuthorizationFilter : AuthorizationFilterBase
{
    protected override string[] RequiredRole => new[] {"admin"};

    public AdminAuthorizationFilter(ITokenService tokenService) : base(tokenService)
    {
    }
}