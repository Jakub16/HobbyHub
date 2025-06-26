using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.RequestHandlers.Tutorials;
using HobbyHub.Application.RequestHandlers.Tutorials.GetTutorials;
using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Tutorials.GetTutorials;

public class GetTutorialsQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ITutorialMapper> _tutorialMapper;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private GetTutorialsQueryHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _tutorialMapper = new Mock<ITutorialMapper>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new GetTutorialsQueryHandler(_repository.Object, _mapper.Object, _tutorialMapper.Object, _log.Object);
    }

    [Test]
    public async Task ShouldHandleGetTutorialsQuery()
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
        
        _repository.Setup(repository => repository.TutorialsRepository
                .GetAllTutorials(CancellationToken.None))
            .ReturnsAsync(tutorials);

        _repository.Setup(repository => repository.UsersRepository
                .GetUsersByIds(It.IsAny<List<int>>(), CancellationToken.None))
            .ReturnsAsync([]);
        
        _mapper.Setup(mapper => mapper.Map<UserSummaryResponse>(It.IsAny<User>()))
            .Returns(userSummary);
        
        _tutorialMapper.Setup(tutorialMapper => tutorialMapper
                .Map(tutorials, It.IsAny<Dictionary<int, UserSummaryResponse>>()))
            .Returns(tutorialResponses);
        
        var result = await _sut.Handle(new GetTutorialsQuery(), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(tutorialResponses);
        result.Outcome.Count.Should().Be(tutorialResponses.Count);
    }
}