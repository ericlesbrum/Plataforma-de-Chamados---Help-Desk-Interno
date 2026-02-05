namespace Web_API.Application.DTOs.Usuarios;

public class UsuarioInfoDto
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public bool Ativo { get; set; }
    public string? PerfilUsuario { get; set; }
}
