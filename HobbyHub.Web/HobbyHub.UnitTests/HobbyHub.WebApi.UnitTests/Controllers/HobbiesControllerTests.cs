using FluentAssertions;
using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.CreateFavoriteHobby;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.DeleteFavoriteHobby;
using HobbyHub.Application.RequestHandlers.Hobbies.PredefinedHobbies.CreatePredefinedHobby;
using HobbyHub.Application.RequestHandlers.Hobbies.PredefinedHobbies.GetPredefinedHobbies;
using HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.CreateUserHobby;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Hobbies;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace HobbyHub.WebApi.UnitTests.Controllers;

public class HobbiesControllerTests
{
    private Mock<IHobbyHubRequestHandler> _requestHandler;
    private HobbiesController _sut;

    private static readonly Error NotFoundError = new Error("test", "test", StatusCodes.Status404NotFound);
    private static readonly Error BadRequestError = new Error("test", "test", StatusCodes.Status400BadRequest);

    [SetUp]
    public void SetUp()
    {
        _requestHandler = new Mock<IHobbyHubRequestHandler>();
        _sut = new HobbiesController(_requestHandler.Object);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task CreatePredefinedHobbyShouldReturnProperResult(bool isError)
    {
        var request = new CreateHobbyRequest()
        {
            Name = "name",
            IconType = "icon"
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreatePredefinedHobbyCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.CreatePredefinedHobby(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreatePredefinedHobbyCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreatePredefinedHobbyCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.CreatePredefinedHobby(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreatePredefinedHobbyCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task CreateCustomHobbyShouldReturnProperResult(bool isError)
    {
        var request = new CreateUserHobbyRequest()
        {
            UserId = 1,
            Name = "name",
            IconType = "icon"
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateUserHobbyCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.CreateUserHobby(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateUserHobbyCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateUserHobbyCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.CreateUserHobby(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateUserHobbyCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task CreateFavoriteHobbyShouldReturnProperResult(bool isError)
    {
        var request = new CreateFavoriteHobbyRequest()
        {
            UserId = 1,
            HobbyId = 1,
            IsHobbyPredefined = true
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateFavoriteHobbyCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.CreateFavoriteHobby(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateFavoriteHobbyCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateFavoriteHobbyCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.CreateFavoriteHobby(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateFavoriteHobbyCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetPredefinedHobbiesShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<HobbyResponse>()
        {
            Count = 1,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetPredefinedHobbiesQuery>()))
                .ReturnsAsync(Result<ListResponse<HobbyResponse>>.Failure(BadRequestError));

            var result = await _sut.GetPredefinedHobbies();

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetPredefinedHobbiesQuery>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetPredefinedHobbiesQuery>()))
                .ReturnsAsync(Result<ListResponse<HobbyResponse>>.Success(response));

            var result = await _sut.GetPredefinedHobbies();

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetPredefinedHobbiesQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<HobbyResponse>>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task DeleteFavoriteHobbyShouldReturnProperResult(bool isError)
    {
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteFavoriteHobbyCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.DeleteFavoriteHobby(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteFavoriteHobbyCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteFavoriteHobbyCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.DeleteFavoriteHobby(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteFavoriteHobbyCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse>>();
        }
    }
}