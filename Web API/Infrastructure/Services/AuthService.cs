using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Web_API.Application.DTOs.Usuarios;
using Web_API.Application.Interfaces;
using Web_API.Domain.Entities;
using Web_API.Domain.Interfaces;
using Web_API.Infrastructure.Authentication;

namespace Web_API.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(IUsuarioRepository usuarioRepository,
        IJwtService jwtService,
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
        _configuration = configuration;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginModelDto loginModelDto)
    {
        var usuario =
            await _usuarioRepository.GetByEmailAsync(loginModelDto.Email!)
            ?? throw new SecurityTokenException("Credenciais inválidas");

        if (!usuario.Ativo ||
            !PasswordHasher.Verify(loginModelDto.Senha!, usuario.Senha))
        {
            throw new SecurityTokenException("Credenciais inválidas");
        }

        var claims = BuildClaims(usuario!);

        var accessToken =
            _jwtService.GenerateAccessToken(claims, _configuration);

        var refreshTokenValue = _jwtService.GenerateRefreshToken();

        var refreshTokenMinutes = _configuration.GetValue<int>("JWT:RefreshTokenValidityInMinutes");

        var teste = DateTime.UtcNow.AddMinutes(refreshTokenMinutes);

        var refreshToken = new RefreshToken
        {
            UsuarioId = usuario.Id,
            Token = PasswordHasher.Hash(refreshTokenValue),
            ExpiryDate = DateTime.UtcNow.AddMinutes(refreshTokenMinutes),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return new TokenResponseDto
        {
            Token =
                new JwtSecurityTokenHandler()
                    .WriteToken(accessToken),
            RefreshToken = refreshTokenValue,
        };
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
    {
        var principal = _jwtService
            .GetPrincipalFromExpiredToken(refreshTokenRequestDto.AccessToken!, _configuration);

        var userId = int.Parse(
           principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var storedToken =
            await _refreshTokenRepository
                .GetValidTokenAsync(userId, refreshTokenRequestDto.RefreshToken!);

        if (storedToken == null)
            throw new SecurityTokenException("Refresh token inválido");

        storedToken.IsRevoked = true;

        await _refreshTokenRepository.RevokeAsync(storedToken);

        var usuario = await _usuarioRepository.GetByIdAsync(userId)
            ?? throw new SecurityTokenException("Usuário inválido");

        var newAccessToken =
            _jwtService.GenerateAccessToken(BuildClaims(usuario), _configuration);

        ValidateAccessToken(new JwtSecurityTokenHandler().WriteToken(newAccessToken));

        var newRefreshValue = _jwtService.GenerateRefreshToken();
        var refreshTokenMinutes = _configuration.GetValue<int>("JWT:RefreshTokenValidityInMinutes");

        var newRefreshToken = new RefreshToken
        {
            UsuarioId = usuario.Id,
            Token = PasswordHasher.Hash(newRefreshValue),
            ExpiryDate = DateTime.UtcNow.AddMinutes(refreshTokenMinutes),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken);

        ValidateAccessToken(
            new JwtSecurityTokenHandler().WriteToken(newAccessToken)
        );

        return new TokenResponseDto
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(newAccessToken),
            RefreshToken = newRefreshToken.Token,
        };
    }

    public ClaimsPrincipal ValidateAccessToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        var validationParameters =
            _jwtService.GetValidationParameters(_configuration);

        return handler.ValidateToken(
            token, validationParameters, out _);
    }

    private static IEnumerable<Claim> BuildClaims(UsuarioModelDto usuario)
    {
        return new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.PerfilUsuario.ToString())
        };
    }
}
