using FluentAssertions;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Hobbies.PredefinedHobbies.CreatePredefinedHobby;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Hobbies;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.Application.UnitTests.RequestHandlers.Hobbies.PredefinedHobbies.CreatePredefinedHobby;

public class CreatePredefinedHobbyCommandHandlerTests
{
    private Mock<IHobbyHubRepository> _repository;
    private Mock<ILogger> _log;
    private CreatePredefinedHobbyCommandHandler _sut;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IHobbyHubRepository>();
        _log = new Mock<ILogger>();
        _sut = new CreatePredefinedHobbyCommandHandler(_repository.Object, _log.Object);
    }
    
    [Test]
    public async Task HandleShouldReturnFailureWhenPredefinedHobbyWithIdenticalNameExists()
    {
        var request = new CreatePredefinedHobbyCommand(new CreateHobbyRequest()
        {
            Name = Stub.PredefinedHobby.Name,
            IconType = Stub.PredefinedHobby.IconType
        });
        
        _repository.Setup(x => x.HobbiesRepository.Add(It.IsAny<Hobby>(), CancellationToken.None));
        
        _repository.Setup(x => x.HobbiesRepository.GetHobbyByName(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(new Hobby()
            {
                HobbyId = 1,
                Name = "Predefined Hobby 1"
            });
        
        var result = await _sut.Handle(request, CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        _log.Verify(l => l.Warning(It.IsAny<string>()), Times.Once);
        result.Error.Should().Be(HobbyErrors.HobbyWithTheSameNameAlreadyExist("Predefined Hobby 1", true, 1));
    }

    [Test]
    public async Task ShouldHandleCreatePredefinedHobbyCommand()
    {
        var request = new CreatePredefinedHobbyCommand(new CreateHobbyRequest()
        {
            Name = Stub.PredefinedHobby.Name,
            IconType = Stub.PredefinedHobby.IconType
        });
        
        _repository.Setup(x => x.HobbiesRepository.Add(It.IsAny<Hobby>(), CancellationToken.None));
        
        await _sut.Handle(request, CancellationToken.None);
        _repository.Verify(x => x.HobbiesRepository.Add(It.IsAny<Hobby>(), CancellationToken.None));
    }
}