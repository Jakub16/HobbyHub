using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.UserIdentity.RegisterUser;
using HobbyHub.Contract.Requests.UserIdentity;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.UserIdentity.Provider.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.UserIdentity;

public class RegisterUserCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private Mock<IPasswordHasher> _passwordHasher;
    private RegisterUserCommandHandler _sut;

    private static readonly RegisterUserCommand Command = new RegisterUserCommand(new RegisterUserRequest()
    {
        Email = "email",
        Password = "password"
    });

    private static readonly User User = new User()
    {
        UserId = 1,
        Name = "name",
        Surname = "surname",
        Email = "email"
    };

    private static readonly Database.Entities.UserIdentity UserIdentity = new Database.Entities.UserIdentity
    {
        UserIdentityId = 1,
        UserId = 1,
        Email = "email",
        Password = "password",
        User = User
    };
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _passwordHasher = new Mock<IPasswordHasher>();
        _sut = new RegisterUserCommandHandler(_repository.Object, _passwordHasher.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserWithTheSameEmailExists()
    {
        _repository.Setup(r =>
                r.UserIdentityRepository.GetUserIdentityByEmail(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(UserIdentity);

        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.EmailAlreadyRegistered(Command.Email));
    }
    
    [Test]
    public async Task ShouldHandleRegisterUserCommand()
    {
        _repository.Setup(r =>
                r.UserIdentityRepository.GetUserIdentityByEmail(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync((Database.Entities.UserIdentity)null!);

        _repository.Setup(r => r.UsersRepository.Add(It.IsAny<User>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        _passwordHasher.Setup(p => p.Hash(It.IsAny<string>()))
            .Returns("hashedPassword");

        _repository.Setup(r => r.UserIdentityRepository.Add(It.IsAny<Database.Entities.UserIdentity>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
        _repository.Verify(r => r.UsersRepository.Add(It.IsAny<User>(), CancellationToken.None), Times.Once);
        _repository.Verify(r => r.UserIdentityRepository.Add(It.IsAny<Database.Entities.UserIdentity>(), CancellationToken.None), Times.Once);
    }
}