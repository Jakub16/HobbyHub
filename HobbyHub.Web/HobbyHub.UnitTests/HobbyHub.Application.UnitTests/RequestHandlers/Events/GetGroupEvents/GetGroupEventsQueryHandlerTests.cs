using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Events.GetGroupEvents;
using HobbyHub.Contract.Responses.Events;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Events.GetGroupEvents;

public class GetGroupEventsQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private GetGroupEventsQueryHandler _sut;
    
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

    private static readonly List<Event> Events = new List<Event>
    {
        new Event()
        {
            EventId = 1,
            Title = "event1",
            Description = "description1",
            Users = Users,
            CreatedBy = 1
        },
        new Event()
        {
            EventId = 2,
            Title = "event2",
            Description = "description2",
            Users = Users,
            CreatedBy = 1
        }
    };
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new GetGroupEventsQueryHandler(_repository.Object, _mapper.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenGroupDoesNotExist()
    {
        _repository.Setup(r => r.GroupsRepository.GroupExists(1, CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new GetGroupEventsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(GroupErrors.GroupNotFound(1));
    }

    [Test]
    public async Task ShouldHandleGetGroupEventsQuery()
    {
        var usersSummaryResponse = new List<UserSummaryResponse>()
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
        
        var creator = usersSummaryResponse[0];

        var eventsResponse = new List<EventResponse>()
        {
            new EventResponse()
            {
                EventId = 1,
                Title = "event1",
                Description = "description1",
                CreatedBy = creator,
                Users = usersSummaryResponse
            },
            new EventResponse()
            {
                EventId = 2,
                Title = "event2",
                Description = "description2",
                CreatedBy = creator,
                Users = usersSummaryResponse
            }
        };
        
        _repository.Setup(r => r.GroupsRepository.GroupExists(1, CancellationToken.None))
            .ReturnsAsync(true);

        _repository.Setup(r => r.UsersRepository.GetUsersByIds(It.IsAny<List<int>>(), CancellationToken.None))
            .ReturnsAsync(Users);

        _repository.Setup(r => r.EventsRepository.GetGroupEvents(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(Events);

        _mapper.Setup(m => m.Map<UserSummaryResponse>(It.IsAny<User>()))
            .Returns(creator);

        _mapper.Setup(m => m.Map<List<UserSummaryResponse>>(It.IsAny<List<User>>()))
            .Returns(usersSummaryResponse);

        var result = await _sut.Handle(new GetGroupEventsQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Items.Should().BeEquivalentTo(eventsResponse);
        result.Outcome.Count.Should().Be(eventsResponse.Count);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}