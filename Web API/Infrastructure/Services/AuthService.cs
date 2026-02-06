using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Web_API.Application.DTOs.Login;
using Web_API.Application.DTOs.Session;
using Web_API.Application.DTOs.Token;
using Web_API.Application.DTOs.Usuarios;
using Web_API.Application.Interfaces;
using Web_API.Domain.Entities;
using Web_API.Domain.Enums;
using Web_API.Domain.Interfaces;
using Web_API.Infrastructure.Authentication;
using Web_API.Infrastructure.Data;

namespace Web_API.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly HelpDeskContext _context;

    public AuthService(IUsuarioRepository usuarioRepository,
        IJwtService jwtService,
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository,
        HelpDeskContext helpDeskContext)
    {
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
        _configuration = configuration;
        _refreshTokenRepository = refreshTokenRepository;
        _context = helpDeskContext;
    }

    public async Task<TokenResponseDto> LoginAsync(
        LoginModelDto dto, string? ip, string? agent)
    {
        var usuario =
            await _usuarioRepository.GetByEmailAsync(dto.Email!)
            ?? throw new SecurityTokenException("Credenciais inválidas");

        if (!usuario.Ativo ||
            !PasswordHasher.Verify(dto.Senha!, usuario.Senha))
            throw new SecurityTokenException("Credenciais inválidas");

        await _refreshTokenRepository.RevokeAllActiveTokensAsync(usuario.Id);

        return await CreateTokensAsync(usuario, dto.DeviceId, ip, agent);
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(
        RefreshTokenRequestDto dto, string? ip, string? agent)
    {
        var principal = _jwtService
            .GetPrincipalFromExpiredToken(dto.AccessToken!, _configuration);

        var userId = int.Parse(
            principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var storedToken =
            await _refreshTokenRepository.GetValidTokenAsync(userId, dto.RefreshToken!);

        if (storedToken is null)
            throw new SecurityTokenException("Refresh token inválido");

        if (storedToken.DeviceId != dto.DeviceId)
        {
            await _refreshTokenRepository.RevokeAllActiveTokensAsync(userId);
            await LogSecurityEvent(SecurityEventTypeEnum.DeviceMismatch, userId, ip, agent);

            throw new SecurityTokenException(
                "Atividade suspeita detectada. Faça login novamente.");
        }

        var usuario = await _usuarioRepository.GetByIdAsync(userId)
            ?? throw new SecurityTokenException("Usuário inválido");

        await _refreshTokenRepository.RevokeAllActiveTokensAsync(userId);

        return await CreateTokensAsync(usuario, dto.DeviceId, ip, agent);
    }

    public async Task<IEnumerable<SessionResponseDto>> GetSessionsAsync(
        int userId, string? currentDeviceId)
    {
        var sessions = await _refreshTokenRepository.GetActiveSessionsAsync(userId);

        return sessions.Select(s => new SessionResponseDto
        {
            SessionId = s.Id,
            DeviceId = s.DeviceId,
            IpAddress = s.IpAddress,
            UserAgent = s.UserAgent,
            CreatedAt = s.CreatedAt,
            ExpiryDate = s.ExpiryDate,
            IsCurrent = s.DeviceId == currentDeviceId
        });
    }

    public async Task RevokeSessionAsync(int userId, int sessionId)
        => await _refreshTokenRepository.RevokeSessionAsync(userId, sessionId);

    public async Task LogoutAsync(int userId) 
        =>await _refreshTokenRepository.RevokeAllActiveTokensAsync(userId);

    private async Task<TokenResponseDto> CreateTokensAsync(
        UsuarioModelDto usuario,
        string? deviceId,
        string? ip,
        string? agent)
    {
        var accessToken =
            _jwtService.GenerateAccessToken(
                BuildClaims(usuario), _configuration);

        var refreshValue = _jwtService.GenerateRefreshToken();
        var refreshMinutes =
            _configuration.GetValue<int>("JWT:RefreshTokenValidityInMinutes");

        var refreshToken = new RefreshToken
        {
            UsuarioId = usuario.Id,
            Token = PasswordHasher.Hash(refreshValue),
            ExpiryDate = DateTime.UtcNow.AddMinutes(refreshMinutes),
            IpAddress = ip,
            UserAgent = agent,
            DeviceId = deviceId
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new TokenResponseDto
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(accessToken),
            RefreshToken = refreshValue
        };
    }

    private async Task LogSecurityEvent(
       SecurityEventTypeEnum type, int userId, string? ip, string? agent)
    {
        _context.SecurityEvents.Add(new SecurityEvent
        {
            UsuarioId = userId,
            EventType = type,
            IpAddress = ip,
            UserAgent = agent
        });

        await _context.SaveChangesAsync();
    }

    private static IEnumerable<Claim> BuildClaims(UsuarioModelDto usuario)
        => new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.PerfilUsuario.ToString())
        };
}
