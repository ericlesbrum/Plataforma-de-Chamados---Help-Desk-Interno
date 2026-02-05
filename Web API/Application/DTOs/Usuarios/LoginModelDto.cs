using System.ComponentModel.DataAnnotations;

namespace Web_API.Application.DTOs.Usuarios;

public class LoginModelDto
{
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória")]
    public string? Senha { get; set; }
}
