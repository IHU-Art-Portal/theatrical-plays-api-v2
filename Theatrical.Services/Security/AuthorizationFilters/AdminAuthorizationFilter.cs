using Theatrical.Services.Security.Jwt;

namespace Theatrical.Services.Security.AuthorizationFilters;

public class AdminAuthorizationFilter : AuthorizationFilterBase
{
    protected override string[] RequiredRole => new[] {"admin"};

    public AdminAuthorizationFilter(ITokenService tokenService) : base(tokenService)
    {
    }
}