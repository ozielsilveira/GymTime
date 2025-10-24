using System.IdentityModel.Tokens.Jwt;
using GymTime.Api.Controllers;
using GymTime.Application.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace GymTime.Api.Tests.Controllers;

public class AuthControllerTests
{
    private static AuthController CreateController(IConfiguration? configuration = null)
    {
        // If no configuration provided, supply default Jwt settings used by tests
        if (configuration == null)
        {
            var defaults = new Dictionary<string, string>
            {
                ["Jwt:SecretKey"] = "DefaultSuperSecretKey1234567890!",
                ["Jwt:Issuer"] = "GymTime",
                ["Jwt:Audience"] = "GymTimeAudience"
            };
            configuration = new ConfigurationBuilder().AddInMemoryCollection(defaults).Build();
        }

        // Create mock logger
        var mockLogger = new Mock<ILogger<AuthController>>();

        return new AuthController(configuration, mockLogger.Object);
    }

    [Fact]
    public void Login_ReturnsBadRequest_WhenRequestIsNull()
    {
        var controller = CreateController();
        var result = controller.Login(null);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("admin", "")]
    [InlineData(" ", "password")]
    public void Login_ReturnsBadRequest_WhenUsernameOrPasswordIsInvalid(string username, string password)
    {
        var controller = CreateController();
        var request = new AuthenticationRequest { Username = username, Password = password };
        var result = controller.Login(request);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        var controller = CreateController();
        var request = new AuthenticationRequest { Username = "user", Password = "wrong" };
        var result = controller.Login(request);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void Login_ReturnsTokenAndExpiry_WhenCredentialsAreValid_WithDefaultConfig()
    {
        var controller = CreateController(); // no Jwt keys in IConfiguration => defaults are used
        var request = new AuthenticationRequest { Username = "admin", Password = "password" };
        var result = controller.Login(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        dynamic body = ok.Value;
        string token = body.Token;
        DateTime expiresAt = body.ExpiresAt;

        Assert.False(string.IsNullOrWhiteSpace(token));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("GymTime", jwt.Issuer);
        Assert.Contains("GymTimeAudience", jwt.Audiences);
        Assert.Equal(expiresAt, jwt.ValidTo);
        Assert.Equal("admin", jwt.Subject);

        // verify claim "role"
        Assert.Contains(jwt.Claims, c => c.Type == "role" && c.Value == "Admin");
    }

    [Fact]
    public void Login_ReturnsToken_WithCustomConfiguration()
    {
        var dict = new Dictionary<string, string>
        {
            ["Jwt:SecretKey"] = "SuperSecretGymTimeKey1234567890!@#$",
            ["Jwt:Issuer"] = "GymTimeIssuer",
            ["Jwt:Audience"] = "GymTimeAudience"
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
        var controller = CreateController(configuration);

        var request = new AuthenticationRequest { Username = "admin", Password = "password" };
        var result = controller.Login(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        dynamic body = ok.Value;
        string token = body.Token;

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        // Assert the values that were provided in the custom configuration
        Assert.Equal("GymTimeIssuer", jwt.Issuer);
        Assert.Contains("GymTimeAudience", jwt.Audiences);
        Assert.Equal("admin", jwt.Subject);
    }
}
