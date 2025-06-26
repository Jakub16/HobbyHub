using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.CreateUserHobby;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Hobbies;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Hobbies.UserHobbies.CreateUserHobby;

public class CreateUserHobbyCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private CreateUserHobbyCommandHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _sut = new CreateUserHobbyCommandHandler(_repository.Object, _log.Object);
    }

    [Test]
    public async Task ShouldHandleCreatePredefinedHobbyCommand()
    {
        var request = new CreateUserHobbyCommand(new CreateUserHobbyRequest()
        {
            UserId = 1,
            Name = "User hobby",
            IconType = "User hobby icon"
        });

        _repository.Setup(x => x.UsersRepository.GetUserByIdAsync(request.UserId, CancellationToken.None))
            .ReturnsAsync(new User()
            {
                UserId = 1,
                Email = "test@email.com",
                Name = "Name",
                Surname = "Surname"
            });
        _repository.Setup(x => x.UserHobbiesRepository.Add(It.IsAny<UserHobby>(), CancellationToken.None));
        _repository.Setup(x => x.HobbiesRepository.GetHobbyByName(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync((Hobby)null!);
        
        await _sut.Handle(request, CancellationToken.None);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
        _repository.Verify(x => x.UserHobbiesRepository.Add(It.IsAny<UserHobby>(), CancellationToken.None));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesntExist()
    {
        var request = new CreateUserHobbyCommand(new CreateUserHobbyRequest()
        {
            UserId = 1,
            Name = "User hobby",
            IconType = "User hobby icon"
        });
        
        _repository.Setup(x => x.UsersRepository.GetUserByIdAsync(request.UserId, CancellationToken.None))
            .ReturnsAsync((User)null!);
        
        _repository.Setup(x => x.UserHobbiesRepository.Add(It.IsAny<UserHobby>(), CancellationToken.None));
        
        var result = await _sut.Handle(request, CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Should().BeEquivalentTo(Result<CommandResponse>.Failure(UserErrors.UserNotFoundCommand(1)));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenHobbyWithIdenticalNameExistsForPredefinedHobby()
    {
        var request = new CreateUserHobbyCommand(new CreateUserHobbyRequest()
        {
            UserId = 1,
            Name = "name",
            IconType = "User hobby icon"
        });

        _repository.Setup(x => x.UsersRepository.GetUserByIdAsync(request.UserId, CancellationToken.None))
            .ReturnsAsync(new User()
            {
                UserId = 1,
                Email = "test@email.com",
                Name = "Name",
                Surname = "Surname"
            });
        
        _repository.Setup(x => x.UserHobbiesRepository.GetUserHobbyByName(It.IsAny<string>(), 1, CancellationToken.None))
            .ReturnsAsync((UserHobby)null!);
        
        _repository.Setup(x => x.HobbiesRepository.GetHobbyByName(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(new Hobby()
            {
                HobbyId = 1,
                Name = "name"
            });
        
        var result = await _sut.Handle(request, CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Should().BeEquivalentTo(Result<CommandResponse>.Failure(HobbyErrors.HobbyWithTheSameNameAlreadyExist("name", true, 1)));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenHobbyWithIdenticalNameExistsForCustomHobby()
    {
        var request = new CreateUserHobbyCommand(new CreateUserHobbyRequest()
        {
            UserId = 1,
            Name = "name",
            IconType = "User hobby icon"
        });

        _repository.Setup(x => x.UsersRepository.GetUserByIdAsync(request.UserId, CancellationToken.None))
            .ReturnsAsync(new User()
            {
                UserId = 1,
                Email = "test@email.com",
                Name = "Name",
                Surname = "Surname"
            });
        
        _repository.Setup(x => x.UserHobbiesRepository.GetUserHobbyByName(It.IsAny<string>(), 1, CancellationToken.None))
            .ReturnsAsync(new UserHobby()
            {
                Name = "name"
            });
        
        _repository.Setup(x => x.HobbiesRepository.GetHobbyByName(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync((Hobby)null!);
        
        var result = await _sut.Handle(request, CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Should().BeEquivalentTo(Result<CommandResponse>.Failure(HobbyErrors.HobbyWithTheSameNameAlreadyExist("name", false, 1)));
    }
}