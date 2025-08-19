using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.BusinessLogic.Services.Interfaces;

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
        var result = await _authService.RegisterUser(
            request.UserName, request.Password,
            request.DisplayName, request.Email);

        if (result.Success)
            return Ok("User registered successfully.");
        return BadRequest(result.Error);
    }

    [HttpPost("log-in")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginUser(
            request.UserName, request.Password);

        if (result.Success)
            return Ok("Logged-in.");
        return Unauthorized(result.Error);
    }

    [Authorize]
    [HttpGet("log-out")]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutUser();
        return Ok("Logged-out");
    }

    [Authorize]
    [HttpGet("check")]
    public IActionResult CheckAuth()
    {
        return Ok("Authenticated");
    }
}
