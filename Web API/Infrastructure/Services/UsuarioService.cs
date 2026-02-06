using Web_API.Application.DTOs.Token;
using Web_API.Application.DTOs.Usuarios;
using Web_API.Domain.Interfaces;

namespace Web_API.Infrastructure.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioResponseDto> AddAsync(RegisterModelDto usuario)
    {
        var response = await _usuarioRepository.AddAsync(usuario);
        return response;
    }

    public async Task<UsuarioResponseDto> UpdateAsync(UsuarioModelDto usuario)
    {
        var response = await _usuarioRepository.UpdateAsync(usuario);
        return response;
    }

    public async Task<UsuarioResponseDto> DeleteAsync(int id)
    {
        var response = await _usuarioRepository.DeleteAsync(id);
        return response;
    }

    public async Task<UsuarioModelDto?> GetByEmailAsync(string email)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        return usuario;
    }

    public async Task<UsuarioModelDto?> GetByIdAsync(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        return usuario;
    }
}
