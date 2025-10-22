using GymTime.Application.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GymTime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Login endpoint that returns a JWT Bearer token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    [ProducesResponseType(400)]
    [AllowAnonymous]
    public IActionResult Login([FromBody] AuthenticationRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Invalid request." });

        var isValid = (request.Username == "admin" && request.Password == "password");
        if (!isValid)
            return Unauthorized(new { message = "Invalid credentials." });

        var jwtKey = _configuration["Jwt:SecretKey"] ?? throw new Exception("jwtKey not found.");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new Exception("jwtIssuer not found.");
        var jwtAudience = _configuration["Jwt:Audience"] ?? throw new Exception("jwtAudience not found.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim(ClaimTypes.Name, request.Username),
            new Claim("role", "Admin")
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new AuthenticationResponse { Token = tokenString, ExpiresAt = token.ValidTo });
    }
}