using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Groups.GetGroupPosts;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Groups.GetGroupPosts;

public class GetGroupPostsQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private GetGroupPostsQueryHandler _sut;
    
    private static readonly Group Group = new Group()
    {
        GroupId = 1,
        Name = "group",
        Description = "description",
        Users = []
    };
    
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
        _log = new Mock<ILogger>();
        _sut = new GetGroupPostsQueryHandler(_repository.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenGroupDoesNotExist()
    {
        _repository.Setup(r => r.GroupsRepository.GetGroupById(1, CancellationToken.None))
            .ReturnsAsync((Group)null!);

        var result = await _sut.Handle(new GetGroupPostsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(GroupErrors.GroupNotFound(1));
    }

    [Test]
    public async Task ShouldHandleGetGroupPostsQuery()
    {
        var dateTime = DateTime.Now;
        
        var userSummaryResponse = new UserSummaryResponse()
        {
            UserId = 1,
            Name = "name",
            Surname = "surname"
        };

        var groupPosts = new List<GroupPost>()
        {
            new GroupPost()
            {
                GroupPostId = 1,
                Content = "content1",
                Group = Group,
                User = User,
                DateTime = dateTime
            },
            new GroupPost()
            {
                GroupPostId = 2,
                Content = "content2",
                Group = Group,
                User = User,
                DateTime = dateTime
            }
        };

        var groupPostsResponse = new List<GroupPostResponse>()
        {
            new GroupPostResponse()
            {
                GroupPostId = 1,
                Content = "content1",
                DateTime = dateTime,
                UserSummary = userSummaryResponse
            },
            new GroupPostResponse()
            {
                GroupPostId = 2,
                Content = "content2",
                DateTime = dateTime,
                UserSummary = userSummaryResponse
            }
        };
        
        _repository.Setup(r => r.GroupsRepository.GetGroupById(1, CancellationToken.None))
            .ReturnsAsync(Group);

        _repository.Setup(r => r.GroupPostsRepository.GetGroupPosts(1, CancellationToken.None))
            .ReturnsAsync(groupPosts);
        
        var result = await _sut.Handle(new GetGroupPostsQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(groupPostsResponse);
        result.Outcome.Count.Should().Be(groupPostsResponse.Count);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}