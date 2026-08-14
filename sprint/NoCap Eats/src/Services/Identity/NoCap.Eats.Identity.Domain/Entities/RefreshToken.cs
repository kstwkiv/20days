namespace NoCap.Eats.Identity.Domain.Entities;

/// <summary>
/// Persisted refresh token — stores a bcrypt hash, not the raw token.
/// </summary>
public class RefreshToken
{
    public Guid   Id          { get; private set; } = Guid.NewGuid();
    public Guid   UserId      { get; private set; }
    public string TokenHash   { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public bool   IsRevoked   { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    protected RefreshToken() { }

    public RefreshToken(Guid userId, string tokenHash, DateTime expiresAt)
    {
        UserId    = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired  => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive   => !IsRevoked && !IsExpired;

    public void Revoke() => IsRevoked = true;
}
