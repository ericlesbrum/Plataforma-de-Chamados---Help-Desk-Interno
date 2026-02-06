using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web_API.Application.DTOs.Login;
using Web_API.Application.DTOs.Token;
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
    public async Task<IActionResult> Login([FromBody] LoginModelDto loginModelDto)
        => Ok(await _authService.LoginAsync(loginModelDto, GetIp(),GetUserAgent()));

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto refreshRequestDto)
        => Ok(await _authService.RefreshTokenAsync(refreshRequestDto,GetIp(), GetUserAgent()));

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync(GetUserId());
        return NoContent();
    }

    [HttpGet("Sessions")]
    public async Task<IActionResult> GetSessions(
        [FromHeader(Name = "X-Device-Id")] string? deviceId)
    {
        var sessions =
            await _authService.GetSessionsAsync(
                GetUserId(),
                deviceId);

        return Ok(sessions);
    }

    [HttpDelete("sessions/{sessionId:int}")]
    public async Task<IActionResult> RevokeSession(
        int sessionId)
    {
        await _authService.RevokeSessionAsync(
            GetUserId(),
            sessionId);

        return NoContent();
    }

    private string? GetIp()
        => HttpContext.Connection.RemoteIpAddress?.ToString();

    private string? GetUserAgent()
        => HttpContext.Request.Headers.UserAgent.ToString();

    private int GetUserId()
        => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}