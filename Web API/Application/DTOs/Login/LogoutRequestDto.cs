namespace Web_API.Application.DTOs.Login;

public class LogoutRequestDto
{
    public int Id { get; set; }
    public string RefreshToken { get; set; } = null!;
}
