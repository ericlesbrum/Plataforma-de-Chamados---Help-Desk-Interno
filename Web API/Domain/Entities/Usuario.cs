using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_API.Domain.Enums;

namespace Web_API.Domain.Entities;

public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    [MinLength(3)]
    public string Nome { get; set; }
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }
    [Required]
    [MaxLength(32)]
    [MinLength(8)]
    public string SenhaHash { get; set; }
    public PerfilUsuarioEnum PerfilUsuario { get; set; }
    public bool Ativo { get; set; }
}
