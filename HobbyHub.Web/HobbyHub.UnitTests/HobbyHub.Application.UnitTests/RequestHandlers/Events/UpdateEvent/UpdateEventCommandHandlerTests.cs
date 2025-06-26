using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Events.UpdateEvent;
using HobbyHub.Contract.Requests.Events;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Events.UpdateEvent;

public class UpdateEventCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IFileUploader> _s3Uploader;
    private Mock<ILogger> _log;
    private UpdateEventCommandHandler _sut;

    private static readonly UpdateEventCommand Command = new UpdateEventCommand(new UpdateEventRequest()
    {
        EventId = 1,
        Title = "title",
        Description = "description",
        Address = "address",
        DateTime = DateTime.Now,
        MainPicture = new FormFile(new MemoryStream("Test file"u8.ToArray()), 0, 0, "Test file", "Test file.jpg")
    });

    private static readonly Event Event = new Event()
    {
        EventId = 1,
        Title = "event1",
        Description = "description1",
        Users = [],
        CreatedBy = 1
    };
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _s3Uploader = new Mock<IFileUploader>();
        _log = new Mock<ILogger>();
        _sut = new UpdateEventCommandHandler(_repository.Object, _s3Uploader.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenEventDoesNotExist()
    {
        _repository.Setup(r => r.EventsRepository.GetEventById(1, CancellationToken.None))
            .ReturnsAsync((Event)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(EventErrors.EventNotFoundCommand(1));
    }

    [Test]
    public async Task ShouldHandleUpdateEventCommand()
    {
        _repository.Setup(r => r.EventsRepository.GetEventById(1, CancellationToken.None))
            .ReturnsAsync(Event);

        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
        _repository.Verify(r => r.EventsRepository.SaveChanges(CancellationToken.None), Times.Exactly(2));
        _s3Uploader.Verify(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);
    }
}