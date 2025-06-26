using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Events.AddEventMember;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Events.AddEventMember;

public class AddEventMemberCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private AddEventMemberCommandHandler _sut;

    private static readonly User User = new User()
    {
        UserId = 1,
        Name = "name",
        Surname = "surname",
        Email = "test@test.com"
    };
    
    private static readonly Event Event = new Event()
    {
        EventId = 1,
        Title = "event",
        Description = "description",
        Users = []
    };
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _sut = new AddEventMemberCommandHandler(_repository.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(new AddEventMemberCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(1));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenEventDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _repository.Setup(r => r.EventsRepository.GetEventById(1, CancellationToken.None))
            .ReturnsAsync((Event)null!);

        var result = await _sut.Handle(new AddEventMemberCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(EventErrors.EventNotFoundCommand(1));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserIsAlreadyAnEventMember()
    {
        var eventWithAlreadyAddedMember = new Event()
        {
            EventId = 1,
            Title = "event",
            Description = "description",
            Users = [User]
        };
        
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _repository.Setup(r => r.EventsRepository.GetEventById(1, CancellationToken.None))
            .ReturnsAsync(eventWithAlreadyAddedMember);

        var result = await _sut.Handle(new AddEventMemberCommand(1, 1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(EventErrors.UserIsAlreadyAdded(1, 1));
    }

    [Test]
    public async Task ShouldHandleAddEventMemberCommand()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _repository.Setup(r => r.EventsRepository.GetEventById(1, CancellationToken.None))
            .ReturnsAsync(Event);

        var result = await _sut.Handle(new AddEventMemberCommand(1, 1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
        _repository.Verify(r => r.EventsRepository.SaveChanges(CancellationToken.None));
    }
}