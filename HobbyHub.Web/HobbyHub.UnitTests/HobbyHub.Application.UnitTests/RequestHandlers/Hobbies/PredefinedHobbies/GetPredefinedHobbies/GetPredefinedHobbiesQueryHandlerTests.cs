using AutoMapper;
using FluentAssertions;
using HobbyHub.Application.RequestHandlers.Hobbies.PredefinedHobbies.GetPredefinedHobbies;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Hobbies.PredefinedHobbies.GetPredefinedHobbies;

public class GetPredefinedHobbiesQueryHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private Mock<IMapper> _mapper;
    private GetPredefinedHobbiesQueryHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _mapper = new Mock<IMapper>();
        _sut = new GetPredefinedHobbiesQueryHandler(_repository.Object, _log.Object, _mapper.Object);
    }

    [Test]
    public async Task ShouldHandleGetPredefinedHobbiesQuery()
    {
        _repository.Setup(x => x.HobbiesRepository.GetAllHobbies(CancellationToken.None))
            .ReturnsAsync(Stub.PredefinedHobbies);
        
        _mapper.Setup(x => x.Map<List<HobbyResponse>>(Stub.PredefinedHobbies))
            .Returns(Stub.PredefinedHobbiesResponse);

        var result = await _sut.Handle(
            It.IsAny<GetPredefinedHobbiesQuery>(), CancellationToken.None);

        result.Outcome.Count.Should().Be(3);
        result.Outcome.Items.Should().BeEquivalentTo(Stub.PredefinedHobbiesResponse);
    }
}