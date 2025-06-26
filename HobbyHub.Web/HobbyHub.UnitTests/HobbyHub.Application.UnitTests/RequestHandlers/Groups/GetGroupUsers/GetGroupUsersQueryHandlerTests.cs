using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Groups.GetGroupUsers;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Groups.GetGroupUsers;

public class GetGroupUsersQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private GetGroupUsersQueryHandler _sut;
    
    private static readonly Group Group = new Group()
    {
        GroupId = 1,
        Name = "group",
        Description = "description",
        Users = [],
        CreatedBy = 1
    };
    
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new GetGroupUsersQueryHandler(_repository.Object, _log.Object, _mapper.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenGroupDoesNotExist()
    {
        _repository.Setup(r => r.GroupsRepository.GetGroupById(1, CancellationToken.None))
            .ReturnsAsync((Group)null!);

        var result = await _sut.Handle(new GetGroupUsersQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(GroupErrors.GroupNotFound(1));
    }

    [Test]
    public async Task ShouldHandleGetGroupUsersQuery()
    {
        var groupMembers = new List<User>()
        {
            new User()
            {
                UserId = 1,
                Name = "name1",
                Surname = "surname1",
                Email = "test1@test.com"
            },
            new User()
            {
                UserId = 2,
                Name = "name2",
                Surname = "surname2",
                Email = "test2@test.com"
            }
        };
        
        var outcome = new List<UserSummaryResponse>()
        {
            new UserSummaryResponse()
            {
                UserId = 1,
                Name = "name1",
                Surname = "surname1"
            },
            new UserSummaryResponse()
            {
                UserId = 2,
                Name = "name2",
                Surname = "surname2"
            }
        };
        
        _repository.Setup(r => r.GroupsRepository.GetGroupById(1, CancellationToken.None))
            .ReturnsAsync(Group);

        _repository.Setup(r => r.GroupsRepository.GetGroupMembers(1, CancellationToken.None))
            .ReturnsAsync(groupMembers);
        
        _mapper.Setup(mapper => mapper.Map<List<UserSummaryResponse>>(It.IsAny<List<User>>()))
            .Returns(outcome);
        
        var result = await _sut.Handle(new GetGroupUsersQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(outcome);
        result.Outcome.Count.Should().Be(outcome.Count);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}