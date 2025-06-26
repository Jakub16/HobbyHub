using FluentAssertions;
using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Activities.DeleteActivity;
using HobbyHub.Application.RequestHandlers.Activities.EndActivity;
using HobbyHub.Application.RequestHandlers.Activities.StartActivity;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Activities;
using HobbyHub.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace HobbyHub.WebApi.UnitTests.Controllers;

public class ActivitiesControllerTests
{
    private Mock<IHobbyHubRequestHandler> _requestHandler;
    private ActivitiesController _sut;
    
    [SetUp]
    public void SetUp()
    {
        _requestHandler = new Mock<IHobbyHubRequestHandler>();
        _sut = new ActivitiesController(_requestHandler.Object);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task StartActivityShouldReturnProperResult(bool isError)
    {
        var request = new StartActivityRequest()
        {
            HobbyId = 1,
            UserId = 1,
            IsHobbyPredefined = true
        };
        
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<StartActivityCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(new Error("test", "test", StatusCodes.Status404NotFound)));
        
            var result = await _sut.StartActivity(request);
        
            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<StartActivityCommand>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<StartActivityCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));
        
            var result = await _sut.StartActivity(request);
        
            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<StartActivityCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task EndActivityShouldReturnProperResult(bool isError)
    {
        var request = new EndActivityRequest();
        
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<EndActivityCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(new Error("test", "test", StatusCodes.Status404NotFound)));

            var result = await _sut.EndActivity(request);
        
            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<EndActivityCommand>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<EndActivityCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));
        
            var result = await _sut.EndActivity(request);
        
            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<EndActivityCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task DeleteActivityShouldReturnProperResult(bool isError)
    {
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteActivityCommand>()))
                .ReturnsAsync(
                    Result<CommandResponse>.Failure(new Error("test", "test", StatusCodes.Status404NotFound)));

            var result = await _sut.DeleteActivity(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteActivityCommand>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteActivityCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.DeleteActivity(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteActivityCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse>>();
        }
    }
}