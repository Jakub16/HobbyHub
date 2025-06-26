using FluentAssertions;
using HobbyHub.Application.RequestHandlers.Tutorials.SearchTutorials;
using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.Database.Entities;
using Moq;
using NUnit.Framework;
using Serilog;
using AutoMapper;
using HobbyHub.Application.RequestHandlers.Tutorials;
using HobbyHub.Contract.Requests.Tutorials;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using HobbyHub.Repository.Abstractions.Tutorials;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Tutorials.SearchTutorials;

public class SearchTutorialsQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ITutorialMapper> _tutorialMapper;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private SearchTutorialsQueryHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _tutorialMapper = new Mock<ITutorialMapper>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new SearchTutorialsQueryHandler(_repository.Object, _mapper.Object, _tutorialMapper.Object, _log.Object);
    }

    [Test]
    public async Task ShouldHandleSearchTutorialsQuery()
    {
        var tutorials = new List<Tutorial>
        {
            new Tutorial { TutorialId = 1, Title = "title1", Description = "description1", Category = "category1"},
            new Tutorial { TutorialId = 2, Title = "title2", Description = "description2", Category = "category2" }
        };

        var tutorialsWithPaging = new TutorialsWithPaging()
        {
            Tutorials = tutorials,
            TotalRecords = 2,
            TotalPages = 1
        };
        
        var tutorialResponses = new List<TutorialResponse>
        {
            new TutorialResponse { TutorialId = 1, Title = "title 1", Description = "description1", Category = "category1" },
            new TutorialResponse { TutorialId = 2, Title = "title 2", Description = "description2", Category = "category2" }
        };

        var filters = new SearchTutorialsFilters()
        {
            Keyword = "test",
            PageNumber = 1,
            PageSize = 20
        };

        var user = new UserSummaryResponse()
        {
            UserId = 1,
            Name = "name1",
            Surname = "surname1"
        };

        _repository.Setup(repository => repository.TutorialsRepository
                .SearchTutorials(filters, CancellationToken.None))
            .ReturnsAsync(tutorialsWithPaging);
        
        _mapper.Setup(mapper => mapper.Map<UserSummaryResponse>(It.IsAny<User>()))
            .Returns(user);
        
        _repository.Setup(repository => repository.UsersRepository
                .GetUsersByIds(It.IsAny<List<int>>(), CancellationToken.None))
            .ReturnsAsync([]);
        
        _tutorialMapper.Setup(tutorialMapper => tutorialMapper
                .Map(tutorials, It.IsAny<Dictionary<int, UserSummaryResponse>>()))
            .Returns(tutorialResponses);

        var result = await _sut.Handle(new SearchTutorialsQuery(filters), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Outcome.Data.Items.Should().BeEquivalentTo(tutorialResponses);
        result.Outcome.Data.Count.Should().Be(tutorialResponses.Count);
        result.Outcome.PageNumber.Should().Be(1);
        result.Outcome.PageSize.Should().Be(20);
    }
}