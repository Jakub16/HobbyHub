using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Groups.CreateOrUpdateGroupMainPicture;
using HobbyHub.Contract.Requests.Groups;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Groups.CreateOrUpdateGroupMainPicture;

public class CreateOrUpdateGroupMainPictureCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IFileUploader> _s3Uploader;
    private Mock<ILogger> _log;
    private CreateOrUpdateGroupMainPictureCommandHandler _sut;

    private static readonly Group Group = new Group()
    {
        GroupId = 1,
        Name = "group",
        Description = "description",
        Users = []
    };
    
    private static readonly CreateOrUpdateGroupMainPictureCommand Command = new CreateOrUpdateGroupMainPictureCommand(new CreateOrUpdateGroupMainPictureRequest() 
    {
        GroupId = 1,
        Picture = new FormFile(new MemoryStream("Test file"u8.ToArray()), 0, 0, "Test file", "Test file.jpg")
    });
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _s3Uploader = new Mock<IFileUploader>();
        _log = new Mock<ILogger>();
        _sut = new CreateOrUpdateGroupMainPictureCommandHandler(_repository.Object, _s3Uploader.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenGroupDoesNotExist()
    {
        _repository.Setup(r => r.GroupsRepository.GetGroupById(1, CancellationToken.None))
            .ReturnsAsync((Group)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(GroupErrors.GroupNotFoundCommand(1));
    }
    
    [Test]
    public async Task ShouldHandleCreateOrUpdateGroupMainPictureCommand()
    {
        _repository.Setup(r => r.GroupsRepository.GetGroupById(1, CancellationToken.None))
            .ReturnsAsync(Group);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Outcome.Id.Should().Be(1);
        _repository.Verify(r => r.GroupsRepository.SaveChanges(CancellationToken.None), Times.Once);
        _s3Uploader.Verify(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);
    }
}