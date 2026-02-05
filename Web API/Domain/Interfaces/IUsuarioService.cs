using Web_API.Application.DTOs.Usuarios;

namespace Web_API.Domain.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDto> AddAsync(RegisterModelDto usuario);
        Task<UsuarioResponseDto> UpdateAsync(UsuarioModelDto usuario);
        Task<UsuarioResponseDto> DeleteAsync(int id);
        Task<UsuarioModelDto?> GetByEmailAsync(string email);
        Task<UsuarioModelDto?> GetByIdAsync(int id);
    }
}
