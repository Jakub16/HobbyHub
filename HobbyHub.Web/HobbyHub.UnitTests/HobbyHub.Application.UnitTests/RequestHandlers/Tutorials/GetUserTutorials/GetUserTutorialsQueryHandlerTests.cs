using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Tutorials;
using HobbyHub.Application.RequestHandlers.Tutorials.GetUserTutorials;
using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Tutorials.GetUserTutorials;

public class GetUserTutorialsQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ITutorialMapper> _tutorialMapper;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private GetUserTutorialsQueryHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _tutorialMapper = new Mock<ITutorialMapper>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new GetUserTutorialsQueryHandler(_repository.Object, _tutorialMapper.Object, _mapper.Object, _log.Object);
    }

    [Test]
    public async Task ShouldHandleGetUserTutorialsQuery()
    {
        var tutorials = new List<Tutorial>
        {
            new Tutorial { TutorialId = 1, Title = "title1", Description = "description1", Category = "category1"},
            new Tutorial { TutorialId = 2, Title = "title2", Description = "description2", Category = "category2" }
        };

        var tutorialResponses = new List<TutorialResponse>
        {
            new TutorialResponse { TutorialId = 1, Title = "title 1", Description = "description1", Category = "category1" },
            new TutorialResponse { TutorialId = 2, Title = "title 2", Description = "description2", Category = "category2" }
        };
        
        var userSummary = new UserSummaryResponse()
        {
            UserId = 1,
            Name = "name",
            Surname = "surname"
        };
        
        _repository.Setup(repository => repository.UsersRepository
                .UserExists(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(true);
        
        _repository.Setup(repository => repository.TutorialsRepository
                .GetUserTutorials(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(tutorials);

        _repository.Setup(repository => repository.UsersRepository
                .GetUsersByIds(It.IsAny<List<int>>(), CancellationToken.None))
            .ReturnsAsync([]);
        
        _mapper.Setup(mapper => mapper.Map<UserSummaryResponse>(It.IsAny<User>()))
            .Returns(userSummary);
        
        _tutorialMapper.Setup(tutorialMapper => tutorialMapper
                .Map(tutorials, It.IsAny<Dictionary<int, UserSummaryResponse>>()))
            .Returns(tutorialResponses);
        
        var result = await _sut.Handle(new GetUserTutorialsQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(tutorialResponses);
        result.Outcome.Count.Should().Be(tutorialResponses.Count);
    }
    
    [Test]
    public async Task ShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(r => r.UsersRepository
                .UserExists(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new GetUserTutorialsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.UserNotFound(1));
    }
}