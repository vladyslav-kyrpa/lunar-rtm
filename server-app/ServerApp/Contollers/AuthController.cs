using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Services.Interfaces;

namespace ServerApp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    public record RegisterRequest(string UserName, string Password, string Email, string DisplayName);
    public record LoginRequest(string UserName, string Password);

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        await _authService.RegisterUser(
            request.UserName, request.Password,
            request.DisplayName, request.Email);

        return Ok("User registered successfully.");
    }

    [HttpPost("log-in")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var isAuthenticated = await _authService.LoginUser(
            request.UserName, request.Password);
        if(isAuthenticated)
            return Ok("Login successful.");
        return Unauthorized("Invalid username or password");
    }

    [Authorize]
    [HttpGet("log-out")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutUser();
        return Ok("Logged-out");
    }
}
