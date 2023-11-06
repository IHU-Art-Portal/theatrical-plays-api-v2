namespace Theatrical.Dto.UsersDtos;

public record JwtOptions(string Issuer,
    string Audience,
    string SigningKey);