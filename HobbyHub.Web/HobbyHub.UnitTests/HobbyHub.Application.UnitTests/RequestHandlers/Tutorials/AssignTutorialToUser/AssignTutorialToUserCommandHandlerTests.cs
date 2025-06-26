using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Tutorials.AssignTutorialToUser;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Tutorials.AssignTutorialToUser;

public class AssignTutorialToUserCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private AssignTutorialToUserCommandHandler _sut;
    
    private static readonly AssignTutorialToUserCommand Command = new AssignTutorialToUserCommand(1, 2);
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _sut = new AssignTutorialToUserCommandHandler(_repository.Object, _log.Object);
    }
    
    [Test]
    public async Task ShouldHandleAssignTutorialToUserCommand()
    {
        _repository.Setup(repository => repository.UsersRepository
                .GetUserByIdAsync(Command.UserId, CancellationToken.None))
            .ReturnsAsync(new User()
            {
                UserId = 1,
                Name = "name",
                Surname = "surname",
                Email = "email@test.com"
            });
        
        _repository.Setup(repository => repository.TutorialsRepository
                .GetTutorialById(Command.TutorialId, CancellationToken.None))
            .ReturnsAsync(new Tutorial()
            {
                TutorialId = 2,
                Title = "title",
                Category = "category"
            });
        
        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Outcome.Id.Should().Be(Command.TutorialId);
    }
    
    [Test]
    public async Task ShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository
                .GetUserByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(Command.UserId));
    }
    
    [Test]
    public async Task ShouldReturnFailureWhenTutorialDoesNotExist()
    {
        _repository.Setup(repository => repository.UsersRepository
                .GetUserByIdAsync(Command.UserId, CancellationToken.None))
            .ReturnsAsync(new User()
            {
                UserId = 1,
                Name = "name",
                Surname = "surname",
                Email = "email@test.com"
            });
        
        _repository.Setup(repository => repository.TutorialsRepository
                .GetTutorialById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tutorial)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(TutorialErrors.TutorialNotFoundCommand(Command.TutorialId));
    }
}