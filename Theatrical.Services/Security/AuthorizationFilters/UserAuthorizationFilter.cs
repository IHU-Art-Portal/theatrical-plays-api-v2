using Theatrical.Services.Security.Jwt;

namespace Theatrical.Services.Security.AuthorizationFilters;

public class UserAuthorizationFilter : AuthorizationFilterBase
{
    protected override string[] RequiredRole => new[] {"user"};

    public UserAuthorizationFilter(ITokenService tokenService) : base(tokenService)
    {
    }
}