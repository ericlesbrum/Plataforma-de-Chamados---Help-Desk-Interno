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

        var tokens = await _context.RefreshTokens
        .Where(rt =>
            rt.UsuarioId == usuarioId &&
            !rt.IsRevoked &&
            rt.ExpiryDate > utcNow
        )
        .OrderByDescending(rt => rt.ExpiryDate)
        .ToListAsync();

        if (!tokens.Any())
            return null;

        var validToken = tokens.FirstOrDefault(rt =>
            PasswordHasher.Verify(refreshToken, rt.Token)
        );

        return validToken;
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeByTokenAsync(int usuarioId, string refreshToken)
    {
        var tokens = await _context.RefreshTokens
        .Where(rt =>
            rt.UsuarioId == usuarioId &&
            !rt.IsRevoked &&
            rt.ExpiryDate > DateTime.UtcNow
        ).ToListAsync();

        var token = tokens.FirstOrDefault(rt =>
            PasswordHasher.Verify(refreshToken, rt.Token)
        );

        if(token is null)
            return;

        token.IsRevoked = true;

        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllActiveTokensAsync(int usuarioId)
    {
        var tokens = await _context.RefreshTokens
        .Where(rt =>
            rt.UsuarioId == usuarioId &&
            !rt.IsRevoked &&
            rt.ExpiryDate > DateTime.UtcNow
        ).ToListAsync();

        if (!tokens.Any())
            return;

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveSessionsAsync(int usuarioId)
    {
        return await _context.RefreshTokens
            .Where(rt =>
                rt.UsuarioId == usuarioId &&
                !rt.IsRevoked &&
                rt.ExpiryDate > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    }

    public async Task RevokeSessionAsync(int usuarioId, int sessionId)
    {
        var session = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt =>
                rt.Id == sessionId &&
                rt.UsuarioId == usuarioId);

        if (session is null) return;

        session.IsRevoked = true;
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllActiveTokensAsync(int usuarioId, int sessionId)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt =>
                rt.UsuarioId == usuarioId &&
                !rt.IsRevoked &&
                rt.ExpiryDate > DateTime.UtcNow)
            .ToListAsync();

        foreach (var t in tokens)
            t.IsRevoked = true;

        await _context.SaveChangesAsync();
    }
}
