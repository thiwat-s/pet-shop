namespace PetShop.Api.Configuration;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = "";
    public string Audience { get; init; } = "";
    public string Key { get; init; } = "";
    public int ExpiryHours { get; init; } = 8;
}
