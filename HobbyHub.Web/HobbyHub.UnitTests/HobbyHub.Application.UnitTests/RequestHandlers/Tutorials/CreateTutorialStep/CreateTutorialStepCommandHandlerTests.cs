using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Tutorials.CreateTutorialStep;
using HobbyHub.Contract.Requests.Tutorials;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Tutorials.CreateTutorialStep;

public class CreateTutorialStepCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IFileUploader> _s3Uploader;
    private Mock<ILogger> _logger;
    private CreateTutorialStepCommandHandler _sut;
    
    private static readonly CreateTutorialStepCommand Command = new(new CreateTutorialStepRequest()
    {
        TutorialId = 1,
        Title = "title",
        Content = "content",
        StepNumber = 1,
        Pictures = new List<IFormFile>()
        {
            new FormFile(new MemoryStream("Test file"u8.ToArray()), 0, 0, "Test file", "Test file.jpg")
        }
    });

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _s3Uploader = new Mock<IFileUploader>();
        _logger = new Mock<ILogger>();
        _sut = new CreateTutorialStepCommandHandler(_repository.Object, _s3Uploader.Object, _logger.Object);
    }
    
    [Test]
    public async Task ShouldReturnFailureWhenTutorialDoesNotExist()
    {
        _repository.Setup(r => r.TutorialsRepository.GetTutorialById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tutorial)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(TutorialErrors.TutorialNotFoundCommand(1));
    }
    
    [Test]
    public async Task ShouldHandleCreateTutorialStepCommand()
    {
        _repository.Setup(r => r.TutorialsRepository.GetTutorialById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Tutorial()
            {
                Title = "title",
                Category = "category",
                Description = "description"
            });
        
        _repository.Setup(repository => repository
                .Add(It.IsAny<TutorialStep>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        _repository.Setup(r => r.Add(It.IsAny<TutorialStepPicture>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repository.Setup(r => r.TutorialStepPicturesRepository
                .GetTutorialStepPictureById(It.IsAny<int>()))
            .Returns(new TutorialStepPicture());

        var result = await _sut.Handle(Command, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _repository.Verify(r => r.Add(It.IsAny<TutorialStep>(), It.IsAny<CancellationToken>()), Times.Once);
        _logger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
        _s3Uploader.Verify(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);
    }
}