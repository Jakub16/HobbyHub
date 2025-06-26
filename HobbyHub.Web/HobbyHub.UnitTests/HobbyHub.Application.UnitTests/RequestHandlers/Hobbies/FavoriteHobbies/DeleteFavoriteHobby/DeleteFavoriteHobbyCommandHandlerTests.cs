using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.DeleteFavoriteHobby;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Hobbies.FavoriteHobbies.DeleteFavoriteHobby;

public class DeleteFavoriteHobbyCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private DeleteFavoriteHobbyCommandHandler _sut;

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
        _log = new Mock<ILogger>();
        _sut = new DeleteFavoriteHobbyCommandHandler(_repository.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenFavoriteHobbyDoesNotExist()
    {
        _repository.Setup(r => r.FavoriteHobbiesRepository.GetFavoriteHobbyById(1, CancellationToken.None))
            .ReturnsAsync((FavoriteHobby)null!);

        var result = await _sut.Handle(new DeleteFavoriteHobbyCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(HobbyErrors.FavoriteHobbyNotFound(1));
    }

    [Test]
    public async Task ShouldHandleDeleteUserHobbyCommand()
    {
        _repository.Setup(r => r.FavoriteHobbiesRepository.GetFavoriteHobbyById(1, CancellationToken.None))
            .ReturnsAsync(new FavoriteHobby()
            {
                FavoriteHobbyId = 1,
                Hobby = new Hobby()
                {
                    HobbyId = 1,
                    Name = "hobby"
                },
                User = User
            });

        var result = await _sut.Handle(new DeleteFavoriteHobbyCommand(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Id.Should().Be(1);
        _repository.Verify(r => r.FavoriteHobbiesRepository.Delete(1, CancellationToken.None), Times.Once);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}