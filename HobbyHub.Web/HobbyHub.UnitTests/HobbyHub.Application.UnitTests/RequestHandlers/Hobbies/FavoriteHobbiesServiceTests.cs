using FluentAssertions;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Hobbies;

public class FavoriteHobbiesServiceTests
{
    private Mock<IHobbyHubRepository> _repository;
    private FavoriteHobbiesService _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _sut = new FavoriteHobbiesService(_repository.Object);
    }

    [Test]
    public async Task GetUserFavoriteHobbies_ShouldReturnFavoriteHobbies()
    {
        var user = new User()
        {
            UserId = 1,
            Email = "email@test.com",
            Name = "name",
            Surname = "surname"
        };
        
        var favoriteHobbies = new List<FavoriteHobby>
        {
            new FavoriteHobby
            {
                FavoriteHobbyId = 1,
                Hobby = new Hobby { HobbyId = 1, Name = "hobby1", IconType = "icon1" },
                UserHobby = null,
                User = user
            },
            new FavoriteHobby
            {
                FavoriteHobbyId = 2,
                Hobby = null,
                UserHobby = new UserHobby { UserHobbyId = 2, Name = "hobby2", IconType = "icon2" },
                User = user
            }
        };

        var favoriteHobbiesResponse = new List<FavoriteHobbyResponse>()
        {
            new FavoriteHobbyResponse()
            {
                FavoriteHobbyId = 1,
                HobbyId = 1,
                Name = "hobby1",
                IconType = "icon1",
                IsPredefined = true
            },
            new FavoriteHobbyResponse()
            {
                FavoriteHobbyId = 2,
                HobbyId = 2,
                Name = "hobby2",
                IconType = "icon2",
                IsPredefined = false
            }
        };

        _repository.Setup(repository => repository.FavoriteHobbiesRepository.GetUserFavoriteHobbies(1, CancellationToken.None))
            .ReturnsAsync(favoriteHobbies);

        var result = await _sut.GetUserFavoriteHobbies(1, CancellationToken.None);

        result.Count.Should().Be(2);
        result.Should().BeEquivalentTo(favoriteHobbiesResponse);
    }
}