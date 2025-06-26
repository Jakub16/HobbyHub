using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.CreateFavoriteHobby;
using HobbyHub.Contract.Requests.Hobbies;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Hobbies.FavoriteHobbies.CreateFavoriteHobby;

public class CreateFavoriteHobbyCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IFavoriteHobbiesService> _favoriteHobbiesService;
    private Mock<ILogger> _log;
    private CreateFavoriteHobbyCommandHandler _sut;

    private static readonly CreateFavoriteHobbyCommand CommandPredefined = new CreateFavoriteHobbyCommand(
        new CreateFavoriteHobbyRequest()
        {
            HobbyId = 1,
            UserId = 1,
            IsHobbyPredefined = true
        });
    
    private static readonly CreateFavoriteHobbyCommand CommandCustom = new CreateFavoriteHobbyCommand(
        new CreateFavoriteHobbyRequest()
        {
            HobbyId = 1,
            UserId = 1,
            IsHobbyPredefined = false
        });

    private static User User = new User()
    {
        UserId = 1,
        Name = "name",
        Surname = "surname",
        Email = "test@test.com"
    };
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _favoriteHobbiesService = new Mock<IFavoriteHobbiesService>();
        _log = new Mock<ILogger>();
        _sut = new CreateFavoriteHobbyCommandHandler(_repository.Object, _favoriteHobbiesService.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((User)null!);

        var result = await _sut.Handle(CommandPredefined, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFoundCommand(1));
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenHobbyAlreadyAddedToFavorites()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _favoriteHobbiesService.Setup(service => service.GetUserFavoriteHobbies(1, CancellationToken.None))
            .ReturnsAsync([
                new FavoriteHobbyResponse()
                {
                    HobbyId = 1,
                    Name = "name",
                    IsPredefined = true
                }
            ]);

        var result = await _sut.Handle(CommandPredefined, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(HobbyErrors.HobbyAlreadyAddedToFavorites(1, 1, true));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenPredefinedHobbyDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _favoriteHobbiesService.Setup(service => service.GetUserFavoriteHobbies(1, CancellationToken.None))
            .ReturnsAsync([]);
        
        _repository.Setup(r => r.HobbiesRepository.GetHobbyById(1, CancellationToken.None))
            .ReturnsAsync((Hobby)null!);

        var result = await _sut.Handle(CommandPredefined, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(HobbyErrors.HobbyNotFoundCommand(1));
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenCustomHobbyDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _favoriteHobbiesService.Setup(service => service.GetUserFavoriteHobbies(1, CancellationToken.None))
            .ReturnsAsync([]);
        
        _repository.Setup(r => r.UserHobbiesRepository.GetUserHobbyById(1, CancellationToken.None))
            .ReturnsAsync((UserHobby)null!);

        var result = await _sut.Handle(CommandCustom, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(HobbyErrors.HobbyNotFoundCommand(1));
    }

    [Test]
    public async Task ShouldHandleCreateFavoriteHobbyCommand()
    {
        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);
        
        _favoriteHobbiesService.Setup(service => service.GetUserFavoriteHobbies(1, CancellationToken.None))
            .ReturnsAsync([]);
        
        _repository.Setup(r => r.HobbiesRepository.GetHobbyById(1, CancellationToken.None))
            .ReturnsAsync(new Hobby()
            {
                HobbyId = 1,
                Name = "name",
            });
        
        _repository.Setup(r => r.FavoriteHobbiesRepository.Add(It.IsAny<FavoriteHobby>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        var result = await _sut.Handle(CommandPredefined, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}