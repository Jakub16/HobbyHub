using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Activities.GetUserEndedActivities;
using HobbyHub.Contract.Responses.Activities;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Activities.GetUserEndedActivities;

public class GetUserEndedActivitiesQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _logger;
    private GetUserEndedActivitiesQueryHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _logger = new Mock<ILogger>();
        _sut = new GetUserEndedActivitiesQueryHandler(_repository.Object, _logger.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(repository => repository.UsersRepository.UserExists(1, CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new GetUserEndedActivitiesQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFound(1));
    }
    
    [Test]
    public async Task ShouldHandleGetUserEndedActivitiesQuery()
    {
        var now = DateTime.Now;
        
        var activities = new List<Activity>()
        {
            new Activity()
            {
                ActivityId = 1,
                Name = "name1",
                Distance = 1.1,
                IsDistanceAvailable = true,
                StartTime = now,
                EndTime = now,
                Time = new TimeSpan(0, 0, 0, 1),
                Hobby = new Hobby()
                {
                    HobbyId = 1,
                    Name = "hobby1"
                }
            },
            new Activity()
            {
                ActivityId = 2,
                Name = "name2",
                IsDistanceAvailable = false,
                StartTime = now,
                EndTime = now,
                Time = new TimeSpan(0, 0, 0, 1),
                Hobby = new Hobby()
                {
                    HobbyId = 2,
                    Name = "hobby2"
                }
            }
        };

        var activitiesResponse = new List<ActivityResponse>()
        {
            new ActivityResponse()
            {
                ActivityId = 1,
                ActivityName = "name1",
                Distance = 1.1,
                IsDistanceAvailable = true,
                StartTime = now,
                EndTime = now,
                Time = new TimeSpan(0, 0, 0, 1),
                Hobby = new HobbyResponse()
                {
                    HobbyId = 1,
                    Name = "hobby1"
                }
            },
            new ActivityResponse()
            {
                ActivityId = 2,
                ActivityName = "name2",
                IsDistanceAvailable = false,
                StartTime = now,
                EndTime = now,
                Time = new TimeSpan(0, 0, 0, 1),
                Hobby = new HobbyResponse()
                {
                    HobbyId = 2,
                    Name = "hobby2"
                }
            }
        };
        
        _repository.Setup(repository => repository.UsersRepository.UserExists(1, CancellationToken.None))
            .ReturnsAsync(true);
        
        _repository.Setup(repository => repository.ActivitiesRepository.GetUserEndedActivities(1, CancellationToken.None))
            .ReturnsAsync(activities);
        
        var result = await _sut.Handle(new GetUserEndedActivitiesQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(activitiesResponse);
        result.Outcome.Count.Should().Be(activitiesResponse.Count);
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}