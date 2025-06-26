using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Tutorials;
using HobbyHub.Application.RequestHandlers.Tutorials.GetTutorial;
using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Tutorials.GetTutorial;

public class GetTutorialQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ITutorialMapper> _tutorialMapper;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private GetTutorialQueryHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _tutorialMapper = new Mock<ITutorialMapper>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new GetTutorialQueryHandler(_repository.Object, _mapper.Object, _tutorialMapper.Object, _log.Object);
    }

    [Test]
    public async Task ShouldHandleGetTutorialQuery()
    {
        var tutorial = new Tutorial
            { TutorialId = 1, Title = "title1", Description = "description1", Category = "category1" };

        var tutorialList = new List<Tutorial>() { tutorial };

        var tutorialResponse = new TutorialResponse
            { TutorialId = 1, Title = "title 1", Description = "description1", Category = "category1" };
        
        var userSummary = new UserSummaryResponse()
        {
            UserId = 1,
            Name = "name",
            Surname = "surname"
        };
        
        _repository.Setup(repository => repository.TutorialsRepository
                .GetTutorialById(1, CancellationToken.None))
            .ReturnsAsync(tutorial);

        _repository.Setup(repository =>
                repository.UsersRepository.GetUserByIdAsync(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(new User()
            {
                UserId = 1,
                Name = "name",
                Surname = "surname",
                Email = "email@test.com"
            });
        
        _mapper.Setup(mapper => mapper.Map<UserSummaryResponse>(It.IsAny<User>()))
            .Returns(userSummary);
        
        _tutorialMapper.Setup(tutorialMapper => tutorialMapper
                .Map(tutorialList, It.IsAny<Dictionary<int, UserSummaryResponse>>()))
            .Returns([tutorialResponse]);
        
        var result = await _sut.Handle(new GetTutorialQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Should().BeEquivalentTo(tutorialResponse);
    }

    [Test]
    public async Task HandleShouldReturnFailureWhenTutorialDoesNotExist()
    {
        const int tutorialId = 1;

        _repository.Setup(repository => repository.TutorialsRepository
                .GetTutorialById(tutorialId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tutorial)null!);
        
        var result = await _sut.Handle(new GetTutorialQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(TutorialErrors.TutorialNotFound(tutorialId));
    }
}