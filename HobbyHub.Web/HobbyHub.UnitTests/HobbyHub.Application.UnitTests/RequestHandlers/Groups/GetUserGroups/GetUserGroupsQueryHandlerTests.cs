using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Groups.GetUserGroups;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Groups.GetUserGroups;

public class GetUserGroupsQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private GetUserGroupsQueryHandler _sut;
    
    private static readonly User User = new User()
    {
        UserId = 1,
        Name = "name",
        Surname = "surname",
        Email = "test@test.com"
    };
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new GetUserGroupsQueryHandler(_repository.Object, _log.Object, _mapper.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(new GetUserGroupsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFound(1));
    }

    [Test]
    public async Task ShouldHandleGetUserGroupsQuery()
    {
        var userSummaryResponse = new UserSummaryResponse()
        {
            UserId = 1,
            Name = "name1",
            Surname = "surname1"
        };
        
        var userGroups = new List<Group>()
        {
            new Group()
            {
                GroupId = 1,
                Name = "group1",
                Description = "description1",
                CreatedBy = 1
            },
            new Group()
            {
                GroupId = 2,
                Name = "group2",
                Description = "description2",
                IsPrivate = true,
                CreatedBy = 1
            }
        };
        
        var userGroupsResponse = new List<GroupSummaryResponse>()
        {
            new GroupSummaryResponse()
            {
                GroupId = 1,
                Name = "group1",
                Description = "description1",
                CreatedBy = userSummaryResponse
            },
            new GroupSummaryResponse()
            {
                GroupId = 2,
                Name = "group2",
                Description = "description2",
                IsPrivate = true,
                CreatedBy = userSummaryResponse
            }
        };

        var creators = new List<User>()
        {
            new User()
            {
                UserId = 1,
                Name = "name1",
                Surname = "surname1",
                Email = "email1@test.com"
            }
        };
        
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _repository.Setup(r => r.GroupsRepository.GetUserGroups(1, CancellationToken.None))
            .ReturnsAsync(userGroups);
        
        _mapper.Setup(mapper => mapper.Map<UserSummaryResponse>(It.IsAny<User>()))
            .Returns(userSummaryResponse);

        var result = await _sut.Handle(new GetUserGroupsQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(userGroupsResponse);
        result.Outcome.Count.Should().Be(userGroupsResponse.Count);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}