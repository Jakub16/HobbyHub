using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Events.DeleteEvent;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Events.DeleteEvent;

public class DeleteEventCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private DeleteEventCommandHandler _sut;
    
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
        _sut = new DeleteEventCommandHandler(_repository.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenEventDoesNotExist()
    {
        _repository.Setup(r => r.EventsRepository.GetEventById(1, CancellationToken.None))
            .ReturnsAsync((Event)null!);

        var result = await _sut.Handle(new DeleteEventCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(EventErrors.EventNotFoundCommand(1));
    }

    [Test]
    public async Task ShouldHandleDeleteEventCommand()
    {
        _repository.Setup(r => r.EventsRepository.GetEventById(1, CancellationToken.None))
            .ReturnsAsync(Event);

        var result = await _sut.Handle(new DeleteEventCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Outcome.Id.Should().Be(1);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
        _repository.Verify(r => r.EventsRepository.Delete(It.IsAny<Event>(), CancellationToken.None));
    }
}