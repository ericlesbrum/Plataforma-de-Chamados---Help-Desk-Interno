using System.Security.Claims;
using Web_API.Application.DTOs.Usuarios;

namespace Web_API.Domain.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginModelDto loginModelDto);
    Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    ClaimsPrincipal ValidateAccessToken(string token);
}
