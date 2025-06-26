using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Users.GetUserFollows;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using Moq;
using NUnit.Framework;
using Serilog;
using AutoMapper;
using HobbyHub.Repository.Abstractions;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Users.GetUserFollows;

public class GetUserFollowsQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger> _loggerMock;
    private GetUserFollowsQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IHobbyHubRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger>();
        _handler = new GetUserFollowsQueryHandler(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesntExist()
    {
        var query = new GetUserFollowsQuery(1);

        _repositoryMock.Setup(repo => repo.UsersRepository
                .GetUserByIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.UserNotFound(query.UserId));
    }

    [Test]
    public async Task ShouldHandleGetUserFollowsQuery()
    {
        var query = new GetUserFollowsQuery(1);
        var userFollows = new List<User>
        {
            new User { UserId = 2, Email = "test1@test.com", Name = "name1", Surname = "surname1" },
            new User { UserId = 3, Email = "test2@test.com", Name = "name2", Surname = "surname2" }
        };
        var userSummaryResponses = new List<UserSummaryResponse>
        {
            new UserSummaryResponse { UserId = 2, Name = "name1", Surname = "surname1" },
            new UserSummaryResponse { UserId = 3, Name = "name2", Surname = "surname2" }
        };

        _repositoryMock.Setup(repo => repo.UsersRepository
                .UserExists(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _repositoryMock.Setup(repo => repo.UsersRepository
                .GetUserFollows(1, CancellationToken.None))
            .ReturnsAsync(userFollows);
        
        _mapperMock.Setup(mapper => mapper.Map<List<UserSummaryResponse>>(userFollows))
            .Returns(userSummaryResponses);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(userSummaryResponses);
        result.Outcome.Count.Should().Be(userSummaryResponses.Count);
    }
}