using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.BusinessLogic.Services.Interfaces;

namespace ServerApp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public record RegisterRequest(string UserName, string Password, string Email, string DisplayName);
    public record LoginRequest(string UserName, string Password);

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterUser(
            request.UserName, request.Password,
            request.DisplayName, request.Email);

        if (result.IsFailed)
        {
            _logger.LogWarning("Failed user registration attempt: @{Error}", result.Error);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("User @{user} registered", request.UserName);
        return Ok("User registered successfully.");
    }

    [HttpPost("log-in")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginUser(
            request.UserName, request.Password);

        if (result.IsFailed)
        {
            _logger.LogError("Failed user authentication attempt: @{Error}", result.Error);
            return Unauthorized(result.Error);
        }

        _logger.LogError("User @{User} logged-in", request.UserName);
        return Ok("Logged-in");
    }

    [Authorize]
    [HttpGet("log-out")]
    public async Task<IActionResult> Logout()
    {
        var user = GetUsername(User);
        await _authService.LogoutUser();
        _logger.LogInformation("User @{User} logged-out", user);

        return Ok("Logged-out");
    }

    [Authorize]
    [HttpGet("check")]
    public IActionResult CheckAuth()
    {
        _logger.LogInformation("User @{User} checked authentication",
            GetUsername(User));
        return Ok("Authenticated");
    }

    private static string GetUsername(ClaimsPrincipal? user) {
        return user?.Identity?.Name ?? "none";
    }
}
