namespace Web_API.Application.DTOs.Usuarios;

public class TokenResponseDto
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
