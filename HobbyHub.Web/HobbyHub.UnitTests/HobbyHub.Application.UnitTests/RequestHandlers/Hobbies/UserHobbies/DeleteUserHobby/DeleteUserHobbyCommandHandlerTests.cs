using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.DeleteUserHobby;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Hobbies.UserHobbies.DeleteUserHobby;

public class DeleteUserHobbyCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _logger;
    private DeleteUserHobbyCommandHandler _sut;
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _logger = new Mock<ILogger>();
        _sut = new DeleteUserHobbyCommandHandler(_repository.Object, _logger.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.UserExists(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new DeleteUserHobbyCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(1));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserHobbyDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.UserExists(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(true);

        _repository.Setup(r => r.UserHobbiesRepository.UserHobbyExists(1, CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new DeleteUserHobbyCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(HobbyErrors.HobbyNotFoundCommand(1));
    }

    [Test]
    public async Task ShouldHandleDeleteUserHobbyCommand()
    {
        _repository.Setup(r => r.UsersRepository.UserExists(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(true);

        _repository.Setup(r => r.UserHobbiesRepository.UserHobbyExists(1, CancellationToken.None))
            .ReturnsAsync(true);

        var result = await _sut.Handle(new DeleteUserHobbyCommand(1, 1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _repository.Verify(r => r.UserHobbiesRepository.Delete(1, CancellationToken.None), Times.Once);
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}