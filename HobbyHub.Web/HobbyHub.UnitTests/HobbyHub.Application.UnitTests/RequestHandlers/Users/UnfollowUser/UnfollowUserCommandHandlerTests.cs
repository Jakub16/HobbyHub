using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Users.UnfollowUser;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Users;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Users.UnfollowUser;

public class UnfollowUserCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private UnfollowUserCommandHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _sut = new UnfollowUserCommandHandler(_repository.Object, _log.Object);
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        var command = new UnfollowUserCommand(new UnfollowUserRequest() { UserId = 1, UserToUnfollowId = 2 });

        _repository.Setup(repo => repo.UsersRepository
                .UserExists(command.UserId, CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(command.UserId));
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenUserIsNotFollowing()
    {
        var command = new UnfollowUserCommand(new UnfollowUserRequest() { UserId = 1, UserToUnfollowId = 2 });

        _repository.Setup(repo => repo.UsersRepository
                .UserExists(command.UserId, CancellationToken.None))
            .ReturnsAsync(true);
        
        _repository.Setup(repo => repo.UsersRepository
                .IsFollowing(command.UserId, command.UserToUnfollowId, CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.UserIsNotFollowed(command.UserId, command.UserToUnfollowId));
    }

    [Test]
    public async Task ShouldHandleUnfollowUserCommand()
    {
        var command = new UnfollowUserCommand(new UnfollowUserRequest() { UserId = 1, UserToUnfollowId = 2 });

        _repository.Setup(repository => repository.UsersRepository.UserExists(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _repository.Setup(repository => repository.UsersRepository.IsFollowing(command.UserId, command.UserToUnfollowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _repository.Setup(repository => repository.UsersRepository.Unfollow(command.UserId, command.UserToUnfollowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Outcome.Should().BeOfType<CommandResponse>();
        result.Outcome.Id.Should().Be(3);
    }
}