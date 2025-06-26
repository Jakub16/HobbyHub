using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Activities.StartActivity;
using HobbyHub.Contract.Requests.Activities;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Activities.StartActivity;

public class StartActivityCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _logger;
    private StartActivityCommandHandler _sut;

    private static readonly StartActivityCommand Command = new StartActivityCommand(new StartActivityRequest()
    {
        UserId = 1,
        HobbyId = 1,
        IsHobbyPredefined = true
    });
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _logger = new Mock<ILogger>();
        _sut = new StartActivityCommandHandler(_repository.Object, _logger.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(repository => repository.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(1));
    }

    [Test]
    public async Task ShouldHandleStartActivityCommand()
    {
        _repository.Setup(repository => repository.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(new User()
            {
                UserId = 1,
                Name = "name",
                Surname = "surname",
                Email = "test@test.com"
            });
        
        _repository.Setup(repository => repository.HobbiesRepository.GetHobbyById(1, CancellationToken.None))
            .ReturnsAsync(new Hobby()
            {
                HobbyId = 1,
                Name = "name"
            });
        
        _repository.Setup(r => r.ActivitiesRepository.Add(It.IsAny<Activity>(), CancellationToken.None))
            .Returns(Task.CompletedTask);
        
        _repository.Setup(r => r.PostsRepository.Add(It.IsAny<Post>(), CancellationToken.None))
            .Returns(Task.CompletedTask);
        
        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}