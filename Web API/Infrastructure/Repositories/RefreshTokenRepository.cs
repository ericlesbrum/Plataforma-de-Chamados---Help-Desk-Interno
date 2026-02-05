using Microsoft.EntityFrameworkCore;
using Web_API.Domain.Entities;
using Web_API.Domain.Interfaces;
using Web_API.Infrastructure.Authentication;
using Web_API.Infrastructure.Data;

namespace Web_API.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly HelpDeskContext _context;

    public RefreshTokenRepository(HelpDeskContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetValidTokenAsync(
        int usuarioId,
        string refreshToken)
    {
        var utcNow = DateTime.UtcNow;

        var token = await _context.RefreshTokens
        .Where(rt =>
            rt.UsuarioId == usuarioId &&
            !rt.IsRevoked &&
            rt.ExpiryDate > utcNow
        )
        .OrderByDescending(rt => rt.CreatedAt)
        .FirstOrDefaultAsync();

        if (token is null)
            return null;

        return PasswordHasher.Verify(refreshToken, token.Token) ? token: null;
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
}
