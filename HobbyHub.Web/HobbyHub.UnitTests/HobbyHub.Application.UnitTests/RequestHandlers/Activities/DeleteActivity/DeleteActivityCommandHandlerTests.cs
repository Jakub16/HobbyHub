using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Activities.DeleteActivity;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Activities.DeleteActivity;

public class DeleteActivityCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _logger;
    private DeleteActivityCommandHandler _sut;
    

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _logger = new Mock<ILogger>();
        _sut = new DeleteActivityCommandHandler(_repository.Object, _logger.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenActivityDoesNotExist()
    {
        _repository.Setup(repository => repository.ActivitiesRepository.GetActivityById(1, CancellationToken.None))
            .ReturnsAsync((Activity)null!);

        var result = await _sut.Handle(new DeleteActivityCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(ActivityErrors.ActivityNotFoundCommand(1));
    }

    [Test]
    public async Task ShouldHandleDeleteActivityCommand()
    {
        var activity = new Activity()
        {
            ActivityId = 1,
            Name = "name"
        };
        
        _repository.Setup(repository => repository.ActivitiesRepository.GetActivityById(1, CancellationToken.None))
            .ReturnsAsync(activity);
        
        var result = await _sut.Handle(new DeleteActivityCommand(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _repository.Verify(r => r.ActivitiesRepository.Delete(activity, CancellationToken.None), Times.Once);
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}