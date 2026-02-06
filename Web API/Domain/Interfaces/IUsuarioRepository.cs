using Web_API.Application.DTOs.Token;
using Web_API.Application.DTOs.Usuarios;

namespace Web_API.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<UsuarioResponseDto> AddAsync(RegisterModelDto usuario);
    Task<UsuarioResponseDto> UpdateAsync(UsuarioModelDto usuario);
    Task<UsuarioResponseDto> DeleteAsync(int id);
    Task<UsuarioModelDto?> GetByIdAsync(int id);
    Task<UsuarioModelDto?> GetByEmailAsync(string email);

}
