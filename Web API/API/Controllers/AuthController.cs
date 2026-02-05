using Microsoft.AspNetCore.Mvc;
using Web_API.Application.DTOs.Usuarios;
using Web_API.Domain.Interfaces;

namespace Web_API.API.Controllers;

[Route("API/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger,
        IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModelDto loginRequest) 
        => Ok(await _authService.LoginAsync(loginRequest));

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto refreshRequest)
        => Ok(await _authService.RefreshTokenAsync(refreshRequest));

}