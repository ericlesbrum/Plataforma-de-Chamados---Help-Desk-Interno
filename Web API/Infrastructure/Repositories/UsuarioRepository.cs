using Microsoft.EntityFrameworkCore;
using Web_API.Application.DTOs.Usuarios;
using Web_API.Domain.Entities;
using Web_API.Domain.Interfaces;
using Web_API.Infrastructure.Authentication;
using Web_API.Infrastructure.Data;

namespace Web_API.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly HelpDeskContext _context;

    public UsuarioRepository(HelpDeskContext context)
    {
        _context = context;
    }

    public async Task<UsuarioResponseDto> AddAsync(RegisterModelDto usuario)
    {
        var usuarioEntity = GetByEmailAsync(usuario.Email!);

        if (usuarioEntity == null)
        {
            var newUsuario = new Usuario
            {
                Nome = usuario.Nome!,
                Email = usuario.Email!,
                Ativo = true,
                DataCriacao = DateTime.UtcNow,
                SenhaHash = PasswordHasher.Hash(usuario.Senha!),
                PerfilUsuario = usuario.PerfilUsuario

            };
            _context.Usuarios.Add(newUsuario);
            await _context.SaveChangesAsync();
            return await new Task<UsuarioResponseDto>(() => new UsuarioResponseDto
            {
                Status = "Success",
                Message = "Usuário registrado com sucesso."
            });
        }
        else
        {
            return await new Task<UsuarioResponseDto>(() => new UsuarioResponseDto
            {
                Status = "Error",
                Message = "Email já está em uso."
            });
        }
    }

    public async Task<UsuarioResponseDto> UpdateAsync(UsuarioModelDto usuario)
    {
        var usuarioEntity = await GetByIdAsync(usuario.Id);
        if (usuarioEntity == null)
        {
            return await new Task<UsuarioResponseDto>(() => new UsuarioResponseDto
            {
                Status = "Error",
                Message = "Usuário não existe."
            });
        }
        else
        {
            _context.Update(usuarioEntity);
            return await new Task<UsuarioResponseDto>(() => new UsuarioResponseDto
            {
                Status = "Success",
                Message = "Usuário atualizado com sucesso."
            });
        }
    }

    public async Task<UsuarioResponseDto> DeleteAsync(int id)
    {
        var usuarioEntity = await GetByIdAsync(id);
        if (usuarioEntity == null)
        {
            return await new Task<UsuarioResponseDto>(() => new UsuarioResponseDto
            {
                Status = "Error",
                Message = "Usuário não existe."
            });
        }
        else
        {
            _context.Remove(usuarioEntity);
            await _context.SaveChangesAsync();
            return await new Task<UsuarioResponseDto>(() => new UsuarioResponseDto
            {
                Status = "Success",
                Message = "Usuário deletado com sucesso."
            });
        }
    }

    public async Task<UsuarioModelDto?> GetByEmailAsync(string email)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        if (usuario == null)
        {
            return null;
        }
        return new UsuarioModelDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Senha = usuario.SenhaHash,
            PerfilUsuario = usuario.PerfilUsuario,
            Ativo = usuario.Ativo
        };
    }

    public async Task<UsuarioModelDto?> GetByIdAsync(int id)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario == null)
        {
            return null;
        }
        return new UsuarioModelDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Senha = usuario.SenhaHash,
            PerfilUsuario = usuario.PerfilUsuario,
            Ativo = usuario.Ativo
        };
    }
}
