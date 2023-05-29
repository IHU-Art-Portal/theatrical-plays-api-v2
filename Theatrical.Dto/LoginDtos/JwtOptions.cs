namespace Theatrical.Dto.LoginDtos;

public record JwtOptions(string Issuer,
    string Audience,
    string SigningKey);