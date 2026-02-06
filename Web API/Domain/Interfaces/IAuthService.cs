using System.Security.Claims;
using Web_API.Application.DTOs.Login;
using Web_API.Application.DTOs.Session;
using Web_API.Application.DTOs.Token;

namespace Web_API.Domain.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginModelDto loginModelDto, string? ip, string? userAgent);
    Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string? ip, string? userAgent);
    Task LogoutAsync(int userId);
    Task<IEnumerable<SessionResponseDto>> GetSessionsAsync(int userId, string? currentDeviceId);
    Task RevokeSessionAsync(int userId, int sessionId);
}
