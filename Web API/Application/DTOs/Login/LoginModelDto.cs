using System.ComponentModel.DataAnnotations;

namespace Web_API.Application.DTOs.Login;

public class LoginModelDto
{
    public string? Email { get; set; }
    public string? Senha { get; set; }
    public string? DeviceId { get; set; }
}
