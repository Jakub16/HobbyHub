using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Groups.GetSuggestedGroups;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Groups.GetSuggestedGroups;

public class GetSuggestedGroupsQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private GetSuggestedGroupsQueryHandler _sut;
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new GetSuggestedGroupsQueryHandler(_repository.Object, _mapper.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository.UserExists(1, CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new GetSuggestedGroupsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFound(1));
    }

    [Test]
    public async Task ShouldHandleGetSuggestedGroupsQuery()
    {
        var userSummaryResponse = new UserSummaryResponse()
        {
            UserId = 1,
            Name = "name1",
            Surname = "surname1"
        };
        
        var suggestedGroups = new List<Group>()
        {
            new Group()
            {
                GroupId = 1,
                Name = "group1",
                Description = "description1",
                CreatedBy = 1
            },
            new Group()
            {
                GroupId = 2,
                Name = "group2",
                Description = "description2",
                IsPrivate = true,
                CreatedBy = 1
            }
        };
        
        var suggestedGroupsResponse = new List<GroupSummaryResponse>()
        {
            new GroupSummaryResponse()
            {
                GroupId = 1,
                Name = "group1",
                Description = "description1",
                CreatedBy = userSummaryResponse
            },
            new GroupSummaryResponse()
            {
                GroupId = 2,
                Name = "group2",
                Description = "description2",
                IsPrivate = true,
                CreatedBy = userSummaryResponse
            }
        };

        var creators = new List<User>()
        {
            new User()
            {
                UserId = 1,
                Name = "name1",
                Surname = "surname1",
                Email = "email1@test.com"
            }
        };

        var favoriteHobbies = new List<FavoriteHobby>()
        {
            new FavoriteHobby()
            {
                FavoriteHobbyId = 1,
                Hobby = new Hobby()
                {
                    HobbyId = 1,
                    Name = "Football"
                },
                User = creators[0]
            },
            new FavoriteHobby()
            {
                FavoriteHobbyId = 2,
                Hobby = new Hobby()
                {
                    HobbyId = 2,
                    Name = "Basketball"
                },
                User = creators[0]
            }
        };
        
        var keywords = new List<string>() { "Football", "Basketball" };
        
        _repository.Setup(r => r.UsersRepository.UserExists(1, CancellationToken.None))
            .ReturnsAsync(true);
        
        _repository.Setup(r => r.GroupsRepository.GetSuggestedGroups(1, keywords, CancellationToken.None))
            .ReturnsAsync(suggestedGroups);

        _repository.Setup(r => r.UsersRepository.GetUsersByIds(It.IsAny<List<int>>(), CancellationToken.None))
            .ReturnsAsync(creators);

        _repository.Setup(r => r.FavoriteHobbiesRepository.GetUserFavoriteHobbies(1, CancellationToken.None))
            .ReturnsAsync(favoriteHobbies);
        
        _mapper.Setup(mapper => mapper.Map<UserSummaryResponse>(It.IsAny<User>()))
            .Returns(userSummaryResponse);

        var result = await _sut.Handle(new GetSuggestedGroupsQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(suggestedGroupsResponse);
        result.Outcome.Count.Should().Be(suggestedGroupsResponse.Count);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}