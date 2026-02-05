namespace Web_API.Application.DTOs.Usuarios;

public class RefreshTokenRequestDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
