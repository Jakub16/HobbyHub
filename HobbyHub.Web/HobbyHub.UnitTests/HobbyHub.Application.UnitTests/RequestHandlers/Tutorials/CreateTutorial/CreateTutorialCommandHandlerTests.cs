using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Tutorials.CreateTutorial;
using HobbyHub.Contract.Requests.Tutorials;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Tutorials.CreateTutorial;

public class CreateTutorialCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IFileUploader> _s3Uploader;
    private Mock<ILogger> _log;
    private CreateTutorialCommandHandler _sut;
    
    private static readonly CreateTutorialCommand Command = new(new CreateTutorialRequest()
    {
        UserId = 1,
        Title = "title",
        Description = "description",
        Price = 100,
        Category = "category",
        MainPicture = new FormFile(new MemoryStream("Test file"u8.ToArray()), 0, 0, "Test file", "Test file.jpg")

    });

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _s3Uploader = new Mock<IFileUploader>();
        _log = new Mock<ILogger>();
        _sut = new CreateTutorialCommandHandler(_repository.Object, _s3Uploader.Object, _log.Object);
    }

    [Test]
    public async Task ShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(Command.UserId));
    }
    
    [Test]
    public async Task ShouldHandleCreateTutorialCommand()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User()
            {
                UserId = 1,
                Email = "test@test.com",
                Name = "Name",
                Surname = "Surname"
            });
        
        _repository.Setup(repository => repository
                .Add(It.IsAny<Tutorial>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repository.Verify(r => r.Add(It.IsAny<Tutorial>(), It.IsAny<CancellationToken>()), Times.Once);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
        _s3Uploader.Verify(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);
    }
}