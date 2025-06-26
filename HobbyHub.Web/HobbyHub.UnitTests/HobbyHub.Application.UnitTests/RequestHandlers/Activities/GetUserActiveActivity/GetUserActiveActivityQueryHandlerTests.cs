using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Activities.GetUserActiveActivity;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Activities.GetUserActiveActivity;

public class GetUserActiveActivityQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _logger;
    private GetUserActiveActivityQueryHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _logger = new Mock<ILogger>();
        
        _sut = new GetUserActiveActivityQueryHandler(_repository.Object, _logger.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(repository => repository.UsersRepository.UserExists(1, CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new GetUserActiveActivityQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFound(1));
    }
    
    [Test]
    public async Task ShouldHandleGetUserActiveActivityQuery()
    {
        _repository.Setup(repository => repository.UsersRepository.UserExists(1, CancellationToken.None))
            .ReturnsAsync(true);
        
        _repository.Setup(repository => repository.ActivitiesRepository.GetUserActiveActivities(1, CancellationToken.None))
            .ReturnsAsync([
                new Activity()
                {
                    ActivityId = 1,
                    Name = "name",
                    Distance = 1.1,
                    IsDistanceAvailable = true,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now,
                    Time = new TimeSpan(0, 0, 0, 1),
                    Hobby = new Hobby()
                    {
                        HobbyId = 1,
                        Name = "name"
                    }
                }
            ]);
        
        var result = await _sut.Handle(new GetUserActiveActivityQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _repository.Verify(r => r.ActivitiesRepository.GetUserActiveActivities(1, CancellationToken.None), Times.Once);
    }
}