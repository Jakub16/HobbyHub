using FluentAssertions;
using NUnit.Framework;

namespace HobbyHub.UserIdentity.Provider.UnitTests;

public class PasswordHasherTests
{
    private PasswordHasher _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new PasswordHasher();
    }

    [Test]
    public void HashPasswordShouldReturnHashedPassword()
    {
        var password = "password123";

        var hashedPassword = _sut.Hash(password);

        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().NotBe(password);
    }

    [Test]
    public void VerifyPasswordShouldReturnTrueForValidPassword()
    {
        var password = "password123";
        var hashedPassword = _sut.Hash(password);

        var result = _sut.Verify(password, hashedPassword);

        result.Should().BeTrue();
    }

    [Test]
    public void VerifyPasswordShouldReturnFalseForInvalidPassword()
    {
        var password = "password123";
        var hashedPassword = _sut.Hash(password);
        var invalidPassword = "wrongpassword";

        var result = _sut.Verify(invalidPassword, hashedPassword);

        result.Should().BeFalse();
    }
}