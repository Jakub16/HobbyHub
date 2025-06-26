using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Groups.AddGroupMember;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Groups.AddGroupMember;

public class AddGroupMemberCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private AddGroupMemberCommandHandler _sut;

    private static readonly User User = new User()
    {
        UserId = 1,
        Name = "name",
        Surname = "surname",
        Email = "test@test.com"
    };
    
    private static readonly Group Group = new Group()
    {
        GroupId = 1,
        Name = "group",
        Description = "description",
        Users = []
    };
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _sut = new AddGroupMemberCommandHandler(_repository.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(new AddGroupMemberCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(1));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenGroupDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _repository.Setup(r => r.GroupsRepository.GetGroupById(1, CancellationToken.None))
            .ReturnsAsync((Group)null!);

        var result = await _sut.Handle(new AddGroupMemberCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(GroupErrors.GroupNotFoundCommand(1));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserIsAlreadyAGroupMember()
    {
        var groupWithAlredyAddedMember = new Group()
        {
            GroupId = 1,
            Name = "group",
            Description = "description",
            Users = [User]
        };
        
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _repository.Setup(r => r.GroupsRepository.GetGroupById(1, CancellationToken.None))
            .ReturnsAsync(groupWithAlredyAddedMember);

        var result = await _sut.Handle(new AddGroupMemberCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(GroupErrors.UserIsAlreadyAdded(1, 1));
    }
    
    [Test]
    public async Task ShouldHandleAddGroupMemberCommand()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _repository.Setup(r => r.GroupsRepository.GetGroupById(1, CancellationToken.None))
            .ReturnsAsync(Group);

        var result = await _sut.Handle(new AddGroupMemberCommand(1, 1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _repository.Verify(r => r.GroupsRepository.SaveChanges(CancellationToken.None), Times.Once);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}