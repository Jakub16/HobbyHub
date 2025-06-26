using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Users.GetUserSummary;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using Moq;
using NUnit.Framework;
using Serilog;
using AutoMapper;
using HobbyHub.Repository.Abstractions;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Users.GetUserSummary;

public class GetUserSummaryQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private GetUserSummaryQueryHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new GetUserSummaryQueryHandler(_repository.Object, _mapper.Object, _log.Object);
    }

    [Test]
    public async Task ShouldHandleGetUserSummaryQuery()
    {
        const int userId = 1;
        var user = new User { UserId = userId, Email = "test@test.com", Name = "name", Surname = "surname" };
        var userSummaryResponse = new UserSummaryResponse { UserId = userId, Name = "name", Surname = "surname" };

        _repository.Setup(repository => repository.UsersRepository
                .GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _mapper.Setup(mapper => mapper.Map<UserSummaryResponse>(user))
            .Returns(userSummaryResponse);

        var result = await _sut.Handle(new GetUserSummaryQuery(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Outcome.Should().Be(userSummaryResponse);
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        const int userId = 1;

        _repository.Setup(repository => repository.UsersRepository
                .GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(new GetUserSummaryQuery(userId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.UserNotFound(userId));
    }
}