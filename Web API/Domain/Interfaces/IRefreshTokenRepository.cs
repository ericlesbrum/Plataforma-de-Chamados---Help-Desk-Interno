using Web_API.Domain.Entities;

namespace Web_API.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetValidTokenAsync(int usuarioId, string refreshToken);
    Task AddAsync(RefreshToken refreshToken);
    Task RevokeAsync(RefreshToken refreshToken);
}
