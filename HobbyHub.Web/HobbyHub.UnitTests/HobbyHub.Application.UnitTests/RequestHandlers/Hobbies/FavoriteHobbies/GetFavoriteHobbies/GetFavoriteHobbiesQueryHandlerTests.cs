using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.GetFavoriteHobbies;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Hobbies.FavoriteHobbies.GetFavoriteHobbies;

public class GetFavoriteHobbiesQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IFavoriteHobbiesService> _favoriteHobbiesService;
    private Mock<ILogger> _log;
    private GetFavoriteHobbiesQueryHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _favoriteHobbiesService = new Mock<IFavoriteHobbiesService>();
        _log = new Mock<ILogger>();
        _sut = new GetFavoriteHobbiesQueryHandler(_repository.Object, _favoriteHobbiesService.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesntExist()
    {
        _repository.Setup(x => x.UsersRepository.UserExists(1, CancellationToken.None))
            .ReturnsAsync(false);
        
        var result = await _sut.Handle(new GetFavoriteHobbiesQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFound(1));
    }
    
    [Test]
    public async Task ShouldHandleGetFavoriteHobbiesQuery()
    {
        var favoriteHobbiesResponse = new List<FavoriteHobbyResponse>()
        {
            new FavoriteHobbyResponse()
            {
                FavoriteHobbyId = 1,
                HobbyId = 1,
                Name = "hobby1",
                IsPredefined = true
            },

            new FavoriteHobbyResponse()
            {
                FavoriteHobbyId = 2,
                HobbyId = 2,
                Name = "hobby2",
                IsPredefined = false
            }
        };
        
        _repository.Setup(x => x.UsersRepository.UserExists(1, CancellationToken.None))
            .ReturnsAsync(true);
        
        _favoriteHobbiesService.Setup(x => x.GetUserFavoriteHobbies(1, CancellationToken.None))
            .ReturnsAsync(favoriteHobbiesResponse);
        
        var result = await _sut.Handle(new GetFavoriteHobbiesQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(favoriteHobbiesResponse);
        result.Outcome.Count.Should().Be(favoriteHobbiesResponse.Count);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}