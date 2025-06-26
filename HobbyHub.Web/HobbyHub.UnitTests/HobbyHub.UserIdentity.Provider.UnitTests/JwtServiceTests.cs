using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using HobbyHub.UserIdentity.Provider.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

namespace HobbyHub.UserIdentity.Provider.UnitTests;

public class JwtServiceTests
{
    private Mock<IOptions<JwtConfig>> _optionsMock;
    private JwtConfig _jwtConfig;
    private JwtService _sut;

    [SetUp]
    public void SetUp()
    {
        _jwtConfig = new JwtConfig
        {
            Key = "b143c45da5a1f4546fc952bhdf565fbfcc7575a072e926e936db785ac0dc855a304552472518fhgj90aad48f9a9e9d3fa616f274f355d10e1f1cd7d30ad609cde",
            Issuer = "issuer",
            Audience = "audience"
        };
        _optionsMock = new Mock<IOptions<JwtConfig>>();
        _optionsMock.Setup(o => o.Value).Returns(_jwtConfig);
        _sut = new JwtService(_optionsMock.Object);
    }

    [Test]
    public void GenerateShouldReturnValidJwtToken()
    {
        var userId = 1;
        var email = "test@test.com";
        var name = "name";
        var surname = "surname";
        var role = "user";

        var token = _sut.Generate(userId, email, name, surname, role);

        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
        principal.Should().NotBeNull();
        principal.HasClaim(c => c.Type == "userId" && c.Value == userId.ToString()).Should().BeTrue();
        principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier && c.Value == email).Should().BeTrue();
        principal.HasClaim(c => c.Type == ClaimTypes.Email && c.Value == email).Should().BeTrue();
        principal.HasClaim(c => c.Type == ClaimTypes.GivenName && c.Value == name).Should().BeTrue();
        principal.HasClaim(c => c.Type == ClaimTypes.Surname && c.Value == surname).Should().BeTrue();
        principal.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == role).Should().BeTrue();
    }

    [Test]
    public void VerifyShouldReturnTrueForValidToken()
    {
        var token = _sut.Generate(1, "test@example.com", "name", "surname", "user");

        var result = _sut.Verify(token);

        result.Should().BeTrue();
    }

    [Test]
    public void VerifyShouldReturnFalseForInvalidToken()
    {
        var invalidToken = "invalidToken";

        var result = _sut.Verify(invalidToken);

        result.Should().BeFalse();
    }
}