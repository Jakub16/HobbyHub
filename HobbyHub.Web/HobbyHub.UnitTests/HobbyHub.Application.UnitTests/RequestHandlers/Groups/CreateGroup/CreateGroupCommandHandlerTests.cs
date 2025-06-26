using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Groups.CreateGroup;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Groups;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Groups.CreateGroup;

public class CreateGroupCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IFileUploader> _s3Uploader;
    private Mock<ILogger> _log;
    private CreateGroupCommandHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _s3Uploader = new Mock<IFileUploader>();
        _log = new Mock<ILogger>();
        _sut = new CreateGroupCommandHandler(_repository.Object, _s3Uploader.Object, _log.Object);
    }

    [Test]
    public async Task ShouldHandleCreateGroupCommand()
    {
        var request = new CreateGroupCommand(new CreateGroupRequest()
        {
            Description = "test",
            Name = "test",
            UserId = 1,
            MainPicture = new FormFile(new MemoryStream("Test file"u8.ToArray()), 0, 0, "Test file", "Test file.jpg")
        });
        
        _repository.Setup(x => x.UsersRepository.GetUserByIdAsync(request.UserId, CancellationToken.None))
            .ReturnsAsync(new User()
            {
                UserId = 1,
                Email = "test@email.com",
                Name = "name",
                Surname = "surname"
            });

        _repository.Setup(x => x.GroupsRepository.Add(It.IsAny<Group>(), CancellationToken.None));
        
        await _sut.Handle(request, CancellationToken.None);
        _repository.Verify(x => x.GroupsRepository.Add(It.IsAny<Group>(), CancellationToken.None));
        _s3Uploader.Verify(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        var request = new CreateGroupCommand(new CreateGroupRequest()
        {
            Description = "test",
            Name = "test",
            UserId = 1
        });
        
        _repository.Setup(x => x.UsersRepository.GetUserByIdAsync(request.UserId, CancellationToken.None))
            .ReturnsAsync((User)null!);

        _repository.Setup(x => x.GroupsRepository.Add(It.IsAny<Group>(), CancellationToken.None));
        
        var result = await _sut.Handle(request, CancellationToken.None);
        result.Should().BeEquivalentTo(Result<CommandResponse>.Failure(UserErrors.UserNotFoundCommand(1)));
    }
}