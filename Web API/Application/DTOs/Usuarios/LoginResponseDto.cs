namespace Web_API.Application.DTOs.Usuarios;

public class LoginResponseDto
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public int RefreshTokenExpiryTimeInMinutes { get; set; }
    public UsuarioInfoDto? Usuario { get; set; }
}
