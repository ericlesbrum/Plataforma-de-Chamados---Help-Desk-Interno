using Web_API.Domain.Enums;

namespace Web_API.Application.DTOs.Usuarios;

public class UsuarioModelDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public PerfilUsuarioEnum PerfilUsuario { get; set; }
    public bool Ativo { get; set; }
}
