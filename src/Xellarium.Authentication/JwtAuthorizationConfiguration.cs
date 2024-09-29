namespace Xellarium.Authentication;

public class JwtAuthorizationConfiguration
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SigningKey { get; init; }
    public required int ExpirationSeconds { get; init; }
}