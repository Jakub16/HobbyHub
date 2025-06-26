using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.UserIdentity.LoginUser;
using HobbyHub.Contract.Requests.UserIdentity;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.UserIdentity.Provider.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.UserIdentity;

public class LoginUserCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private Mock<IJwtService> _jwtService;
    private Mock<IPasswordHasher> _passwordHasher;
    private LoginUserCommandHandler _sut;

    private static readonly LoginUserCommand Command = new LoginUserCommand(new LoginUserRequest()
    {
        Email = "email",
        Password = "password"
    });

    private static readonly Database.Entities.UserIdentity UserIdentity = new Database.Entities.UserIdentity
    {
        UserIdentityId = 1,
        UserId = 1,
        Email = "email",
        Password = "password",
        User = new User()
        {
            UserId = 1,
            Name = "name",
            Surname = "surname",
            Email = "email"
        }
    };
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _jwtService = new Mock<IJwtService>();
        _passwordHasher = new Mock<IPasswordHasher>();
        _sut = new LoginUserCommandHandler(_repository.Object, _log.Object, _jwtService.Object, _passwordHasher.Object);
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenUserIdentityDoesNotExist()
    {
        _repository.Setup(r =>
                r.UserIdentityRepository.GetUserIdentityByEmail(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync((Database.Entities.UserIdentity)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.InvalidEmailOrPassword);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenPasswordIsNotValid()
    {
        _repository.Setup(r =>
                r.UserIdentityRepository.GetUserIdentityByEmail(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(UserIdentity);

        _passwordHasher.Setup(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.InvalidEmailOrPassword);
    }

    [Test]
    public async Task ShouldHandleLoginUserCommand()
    {
        _repository.Setup(r =>
                r.UserIdentityRepository.GetUserIdentityByEmail(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(UserIdentity);

        _passwordHasher.Setup(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);
        
        _jwtService.Setup(j => j.Generate(UserIdentity.UserId, UserIdentity.Email, UserIdentity.User.Name, UserIdentity.User.Surname, "user"))
            .Returns("token");

        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Should().Be("token");
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}