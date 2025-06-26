using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Events.GetEventMembers;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Events.GetEventMembers;

public class GetEventMembersQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private GetEventMembersQueryHandler _sut;
    
    private static readonly List<User> Users = new List<User>()
    {
        new User()
        {
            UserId = 1,
            Name = "name1",
            Surname = "surname1",
            Email = "test1@test.com"
        },
        new User()
        {
            UserId = 2,
            Name = "name2",
            Surname = "surname2",
            Email = "test2@test.com"
        }
    };
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new GetEventMembersQueryHandler(_repository.Object, _mapper.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenEventDoesNotExist()
    {
        _repository.Setup(r => r.EventsRepository.EventExists(1, CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new GetEventMembersQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(EventErrors.EventNotFound(1));
    }

    [Test]
    public async Task ShouldHandleGetEventMembersQuery()
    {
        var usersResponse = new List<UserSummaryResponse>()
        {
            new UserSummaryResponse()
            {
                UserId = 1,
                Name = "name1",
                Surname = "surname1"
            },
            new UserSummaryResponse()
            {
                UserId = 2,
                Name = "name2",
                Surname = "surname2"
            }
        };
        
        _repository.Setup(r => r.EventsRepository.EventExists(1, CancellationToken.None))
            .ReturnsAsync(true);

        _repository.Setup(r => r.EventsRepository.GetEventMembers(1, CancellationToken.None))
            .ReturnsAsync(Users);

        _mapper.Setup(m => m.Map<List<UserSummaryResponse>>(It.IsAny<List<User>>()))
            .Returns(usersResponse);
        
        var result = await _sut.Handle(new GetEventMembersQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(usersResponse);
        result.Outcome.Count.Should().Be(usersResponse.Count);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}