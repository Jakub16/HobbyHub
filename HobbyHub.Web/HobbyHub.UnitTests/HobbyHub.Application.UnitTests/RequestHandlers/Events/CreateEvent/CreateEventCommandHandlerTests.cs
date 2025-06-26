using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Events.CreateEvent;
using HobbyHub.Contract.Requests.Events;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Events.CreateEvent;

public class CreateEventCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IFileUploader> _s3Uploader;
    private Mock<ILogger> _log;
    private CreateEventCommandHandler _sut;

    private static readonly CreateEventCommand Command = new CreateEventCommand(new CreateEventRequest()
    {
        GroupId = 1,
        UserId = 1,
        Title = "title",
        Description = "description",
        Address = "address",
        DateTime = DateTime.Now,
        MainPicture = new FormFile(new MemoryStream("Test file"u8.ToArray()), 0, 0, "Test file", "Test file.jpg")
    });
    
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
        _s3Uploader = new Mock<IFileUploader>();
        _log = new Mock<ILogger>();
        _sut = new CreateEventCommandHandler(_repository.Object, _s3Uploader.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(r => r.GroupsRepository.GetGroupById(1, CancellationToken.None))
            .ReturnsAsync(Group);
        
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(Command, CancellationToken.None);

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

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(GroupErrors.GroupNotFoundCommand(1));
    }

    [Test]
    public async Task ShouldHandleCreateEventCommand()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _repository.Setup(r => r.GroupsRepository.GetGroupById(1, CancellationToken.None))
            .ReturnsAsync(Group);

        _repository.Setup(r => r.EventsRepository.Add(It.IsAny<Event>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var result = await _sut.Handle(Command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
        _repository.Verify(r => r.EventsRepository.SaveChanges(CancellationToken.None));
        _s3Uploader.Verify(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);
    }
}