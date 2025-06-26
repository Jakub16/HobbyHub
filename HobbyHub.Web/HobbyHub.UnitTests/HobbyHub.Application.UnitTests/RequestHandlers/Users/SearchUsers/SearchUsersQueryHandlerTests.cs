using FluentAssertions;
using HobbyHub.Application.RequestHandlers.Users.SearchUsers;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using Moq;
using NUnit.Framework;
using Serilog;
using AutoMapper;
using HobbyHub.Repository.Abstractions;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Users.SearchUsers;

public class SearchUsersQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private SearchUsersQueryHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new SearchUsersQueryHandler(_repository.Object, _mapper.Object, _log.Object);
    }

    [Test]
    public async Task ShouldHandleSearchUsersQuery()
    {
        const string keyword = "test";
        var users = new List<User>
        {
            new User { UserId = 1, Email = "test1@test.com", Name = "name1", Surname = "surname1" },
            new User { UserId = 2, Email = "test2@test.com", Name = "name2", Surname = "surname2" }
        };
        var userSummaryResponses = new List<UserSummaryResponse>
        {
            new UserSummaryResponse { UserId = 1, Name = "name1", Surname = "surname1" },
            new UserSummaryResponse { UserId = 2, Name = "name2", Surname = "surname2" }
        };

        _repository.Setup(repository => repository.UsersRepository
                .UsersSearch(keyword, It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);
        
        _mapper.Setup(mapper => mapper.Map<List<UserSummaryResponse>>(users))
            .Returns(userSummaryResponses);

        var result = await _sut.Handle(new SearchUsersQuery(keyword), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(userSummaryResponses);
        result.Outcome.Count.Should().Be(userSummaryResponses.Count);
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        const string keyword = "none";
        var users = new List<User>();
        var userSummaryResponses = new List<UserSummaryResponse>();

        _repository.Setup(repository => repository.UsersRepository
                .UsersSearch(keyword, CancellationToken.None))
            .ReturnsAsync(users);
        
        _mapper.Setup(mapper => mapper.Map<List<UserSummaryResponse>>(users))
            .Returns(userSummaryResponses);

        var result = await _sut.Handle(new SearchUsersQuery(keyword), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEmpty();
        result.Outcome.Count.Should().Be(0);
    }
}