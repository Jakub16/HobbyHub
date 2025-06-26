using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Events.GetEvent;
using HobbyHub.Contract.Responses.Events;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Events.GetEvent;

public class GetEventQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<IMapper> _mapper;
    private Mock<ILogger> _log;
    private GetEventQueryHandler _sut;
    
    private static readonly User User = new User()
    {
        UserId = 1,
        Name = "name",
        Surname = "surname",
        Email = "test@test.com"
    };
    
    private static readonly Event Event = new Event()
    {
        EventId = 1,
        Title = "event",
        Description = "description",
        Users = [User]
    };
    
    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _mapper = new Mock<IMapper>();
        _log = new Mock<ILogger>();
        _sut = new GetEventQueryHandler(_repository.Object, _mapper.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenEventDoesNotExist()
    {
        _repository.Setup(r => r.EventsRepository.GetEventById(1, CancellationToken.None))
            .ReturnsAsync((Event)null!);

        var result = await _sut.Handle(new GetEventQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(EventErrors.EventNotFound(1));
    }

    [Test]
    public async Task ShouldHandleGetEventQuery()
    {
        var userResponse = new UserSummaryResponse()
        {
            UserId = 1,
            Name = "name",
            Surname = "surname"
        };

        var eventResponse = new EventResponse()
        {
            EventId = 1,
            Title = "event",
            Description = "description",
            CreatedBy = userResponse,
            Users = [userResponse]
        };

        var users = new List<UserSummaryResponse>() { userResponse };
        
        _repository.Setup(r => r.EventsRepository.GetEventById(1, CancellationToken.None))
            .ReturnsAsync(Event);

        _repository.Setup(r => r.UsersRepository.GetUserByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(User);

        _mapper.Setup(m => m.Map<UserSummaryResponse>(It.IsAny<User>()))
            .Returns(userResponse);

        _mapper.Setup(m => m.Map<List<UserSummaryResponse>>(It.IsAny<List<User>>()))
            .Returns(users);

        var result = await _sut.Handle(new GetEventQuery(1), CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Outcome.Should().BeEquivalentTo(eventResponse);
        _log.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
    }
}