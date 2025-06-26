using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.GetUserHobbies;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Hobbies.UserHobbies.GetUserHobbies;

public class GetUserHobbiesQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private Mock<IMapper> _mapper;
    private GetUserHobbiesQueryHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _mapper = new Mock<IMapper>();
        _sut = new GetUserHobbiesQueryHandler(_repository.Object, _log.Object, _mapper.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenUserDoesNotExist()
    {
        _repository.Setup(x => x.UsersRepository.UserExists(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new GetUserHobbiesQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(UserErrors.UserNotFound(1));
    }

    [Test]
    public async Task ShouldHandleGetUserHobbiesQuery()
    {
        var request = new GetUserHobbiesQuery(1);
        
        _repository.Setup(x => 
                x.UserHobbiesRepository.GetUserHobbiesByUserId(1, CancellationToken.None))
            .ReturnsAsync(Stub.UserHobbies);

        _mapper.Setup(x => x.Map<List<HobbyResponse>>(Stub.UserHobbies))
            .Returns(Stub.UserHobbiesResponse);
        
        _repository.Setup(x => x.UsersRepository.UserExists(It.IsAny<int>(), CancellationToken.None))
            .ReturnsAsync(true);

        var result = await _sut.Handle(
            request, CancellationToken.None);

        result.Outcome.Count.Should().Be(3);
        result.Outcome.Items.Should().BeEquivalentTo(Stub.UserHobbiesResponse);
    }
}