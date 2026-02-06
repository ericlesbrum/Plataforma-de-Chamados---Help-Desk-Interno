using Web_API.Application.DTOs.Usuarios;

namespace Web_API.Application.DTOs.Login;

public class LoginResponseDto
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public int RefreshTokenExpiryTimeInMinutes { get; set; }
    public UsuarioInfoDto? Usuario { get; set; }
}
