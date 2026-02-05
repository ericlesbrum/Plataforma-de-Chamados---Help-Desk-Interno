using System.ComponentModel.DataAnnotations;
using Web_API.Domain.Enums;

namespace Web_API.Application.DTOs.Usuarios;

public class RegisterModelDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O campo {2} é obrigatório.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "O campo {1} é obrigatório.")]
    public string? Senha { get; set; }

    [Required(ErrorMessage = "O campo {3} é obrigatório.")]
    public PerfilUsuarioEnum PerfilUsuario { get; set; }
}
