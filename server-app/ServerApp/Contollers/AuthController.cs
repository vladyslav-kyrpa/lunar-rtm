using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    public AuthController(IConfiguration configuration)
    {
        _config = configuration;
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInCredentials credentials)
    {
        if (credentials == null) {
            return BadRequest("There is no credentials");
        }

        var clientId = _config["Authentication:google-client-id"];
        Console.WriteLine(clientId);
        // test token validation logic
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(credentials.Token, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [clientId]
            });

            Console.WriteLine(payload.Email);
            Console.WriteLine(payload.Name);

            string authToken = "some proper token";
            return Ok(authToken);
        }
        catch (InvalidJwtException ex)
        {
            return BadRequest("Token is invalid: " + ex.Message);
        }
    }
}

public class GoogleSignInCredentials
{
    public string Token { get; set; } = string.Empty;
}