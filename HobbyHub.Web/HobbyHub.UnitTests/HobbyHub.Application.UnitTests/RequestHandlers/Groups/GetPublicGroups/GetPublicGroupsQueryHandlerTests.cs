using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.RequestHandlers.Groups.GetGroupUsers;
using HobbyHub.Application.RequestHandlers.Groups.GetPublicGroups;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Groups.GetPublicGroups;

public class GetPublicGroupsQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private GetPublicGroupsQueryHandler _sut;
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _sut = new GetPublicGroupsQueryHandler(_repository.Object, _log.Object);
    }
    
    [Test]
    public async Task ShouldHandleGetGroupUsersQuery()
    {
        var groups = new List<Group>()
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
        
        var grupsResponse = new List<GroupSummaryResponse>()
        {
            new GroupSummaryResponse()
            {
                GroupId = 1,
                Name = "group1",
                Description = "description1",
                CreatedBy = null
            },
            new GroupSummaryResponse()
            {
                GroupId = 2,
                Name = "group2",
                Description = "description2",
                IsPrivate = true,
                CreatedBy = null
            }
        };

        _repository.Setup(r => r.GroupsRepository.GetPublicGroups(CancellationToken.None))
            .ReturnsAsync(groups);
        
        var result = await _sut.Handle(new GetPublicGroupsQuery(), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(grupsResponse);
        result.Outcome.Count.Should().Be(grupsResponse.Count);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}