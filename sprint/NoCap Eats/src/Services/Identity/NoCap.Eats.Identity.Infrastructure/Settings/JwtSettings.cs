namespace NoCap.Eats.Identity.Infrastructure.Settings;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Secret                { get; init; } = default!;
    public string Issuer                { get; init; } = default!;
    public string Audience              { get; init; } = default!;
    public int    AccessTokenExpiryMinutes { get; init; } = 15;
}
