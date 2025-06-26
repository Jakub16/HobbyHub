using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Users.FollowUser;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using Moq;
using NUnit.Framework;
using Serilog;
using HobbyHub.Contract.Requests.Users;
using HobbyHub.Repository.Abstractions;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Users.FollowUser;

public class FollowUserCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private FollowUserCommandHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _sut = new FollowUserCommandHandler(_repository.Object, _log.Object);
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenUserTriesToFollowItself()
    {
        var command = new FollowUserCommand(new FollowUserRequest { UserId = 1, UserToFollowId = 1 });

        _repository.Setup(repository => repository.UsersRepository
                .UserExists(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.CannotFollowItself);
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        var command = new FollowUserCommand(new FollowUserRequest { UserId = 1, UserToFollowId = 2 });

        _repository.Setup(repository => repository.UsersRepository.UserExists(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(command.UserId));
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenUserToFollowDoesNotExist()
    {
        var command = new FollowUserCommand(new FollowUserRequest { UserId = 1, UserToFollowId = 2 });

        _repository.Setup(repository => repository.UsersRepository.UserExists(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repository.Setup(repository => repository.UsersRepository.GetUserByIdAsync(command.UserToFollowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(command.UserToFollowId));
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenUserIsAlreadyFollowed()
    {
        var command = new FollowUserCommand(new FollowUserRequest { UserId = 1, UserToFollowId = 2 });
        var userToFollow = new User { UserId = 2, Email = "test@test.com", Name = "name", Surname = "surname" };

        _repository.Setup(repository => repository.UsersRepository.UserExists(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repository.Setup(repository => repository.UsersRepository.GetUserByIdAsync(command.UserToFollowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userToFollow);
        _repository.Setup(repository => repository.UsersRepository.GetUserFollows(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([userToFollow]);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.UserIsAlreadyFollowed(command.UserId, command.UserToFollowId));
    }

    [Test]
    public async Task ShouldHandleFollowUserCommand()
    {
        var command = new FollowUserCommand(new FollowUserRequest { UserId = 1, UserToFollowId = 2 });
        var userToFollow = new User { UserId = 2, Email = "test@test.com", Name = "name", Surname = "surname" };

        _repository.Setup(repository => repository.UsersRepository.UserExists(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repository.Setup(repository => repository.UsersRepository.GetUserByIdAsync(command.UserToFollowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userToFollow);
        _repository.Setup(repository => repository.UsersRepository.GetUserFollows(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _repository.Setup(repository => repository.UsersRepository.Follow(command.UserId, command.UserToFollowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Outcome.Should().BeOfType<CommandResponse>();
        result.Outcome.Id.Should().Be(1);
    }
}