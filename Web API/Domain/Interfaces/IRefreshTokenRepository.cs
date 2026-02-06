using Web_API.Domain.Entities;

namespace Web_API.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetValidTokenAsync(int usuarioId, string refreshToken);
    Task AddAsync(RefreshToken refreshToken);
    Task RevokeByTokenAsync(int usuarioId, string refreshToken);
    Task RevokeSessionAsync(int usuarioId, int sessionId);
    Task<IEnumerable<RefreshToken>> GetActiveSessionsAsync(int usuarioId);
    Task RevokeAllActiveTokensAsync(int usuarioId);
}