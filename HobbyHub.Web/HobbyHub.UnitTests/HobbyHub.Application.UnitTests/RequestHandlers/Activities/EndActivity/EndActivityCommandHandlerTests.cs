using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Activities.EndActivity;
using HobbyHub.Contract.Requests.Activities;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Activities.EndActivity;

public class EndActivityCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _logger;
    private Mock<IFileUploader> _fileUploader;
    private EndActivityCommandHandler _sut;

    private static readonly EndActivityCommand Command = new EndActivityCommand(new EndActivityRequest()
    {
        ActivityId = 1,
        Pictures = new List<IFormFile>()
        {
            new FormFile(new MemoryStream("Test file"u8.ToArray()), 0, 0, "Test file", "Test file.jpg")
        },
        Notes = new List<NoteRequest>()
        {
            new NoteRequest()
            {
                Content = "content"
            }
        }
    });

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _logger = new Mock<ILogger>();
        _fileUploader = new Mock<IFileUploader>();
        
        _sut = new EndActivityCommandHandler(_repository.Object, _logger.Object, _fileUploader.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenActivityDoesNotExist()
    {
        _repository.Setup(repository => repository.ActivitiesRepository.GetActivityById(1, CancellationToken.None))
            .ReturnsAsync((Activity)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(ActivityErrors.ActivityNotFoundCommand(1));
    }
    
    [Test]
    public async Task ShouldHandleEndActivityCommand()
    {
        var activity = new Activity()
        {
            ActivityId = 1,
            Name = "name"
        };
        
        _repository.Setup(repository => repository.ActivitiesRepository.GetActivityById(1, CancellationToken.None))
            .ReturnsAsync(activity);
        
        _repository.Setup(repository => repository.ActivityPicturesRepository.Add(It.IsAny<ActivityPicture>(), CancellationToken.None))
            .Returns(Task.CompletedTask);
        
        _repository.Setup(repository => repository.NotesRepository.Add(It.IsAny<Note>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        _repository.Setup(repository => repository.ActivityPicturesRepository
                .GetActivityPictureById(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(new ActivityPicture());
        
        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _repository.Verify(r => r.ActivitiesRepository.SaveChanges(CancellationToken.None), Times.Exactly(4));
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
        _fileUploader.Verify(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);
    }
}