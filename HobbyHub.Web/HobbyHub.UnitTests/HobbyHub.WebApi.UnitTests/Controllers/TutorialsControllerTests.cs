using FluentAssertions;
using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Tutorials.AssignTutorialToUser;
using HobbyHub.Application.RequestHandlers.Tutorials.CreateTutorial;
using HobbyHub.Application.RequestHandlers.Tutorials.CreateTutorialStep;
using HobbyHub.Application.RequestHandlers.Tutorials.GetTutorial;
using HobbyHub.Application.RequestHandlers.Tutorials.GetTutorials;
using HobbyHub.Application.RequestHandlers.Tutorials.SearchTutorials;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Tutorials;
using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace HobbyHub.WebApi.UnitTests.Controllers;

public class TutorialsControllerTests
{
    private Mock<IHobbyHubRequestHandler> _requestHandler;
    private TutorialsController _sut;

    private static readonly Error NotFoundError = new Error("test", "test", StatusCodes.Status404NotFound);
    private static readonly Error BadRequestError = new Error("test", "test", StatusCodes.Status400BadRequest);

    [SetUp]
    public void SetUp()
    {
        _requestHandler = new Mock<IHobbyHubRequestHandler>();
        _sut = new TutorialsController(_requestHandler.Object);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task CreateTutorialShouldReturnProperResult(bool isError)
    {
        var request = new CreateTutorialRequest()
        {
            Title = "title",
            Category = "category"
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateTutorialCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.CreateTutorial(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateTutorialCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateTutorialCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.CreateTutorial(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateTutorialCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task CreateTutorialStepShouldReturnProperResult(bool isError)
    {
        var request = new CreateTutorialStepRequest()
        {
            Title = "title",
            Content = "content"
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateTutorialStepCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.CreateTutorialStep(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateTutorialStepCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateTutorialStepCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.CreateTutorialStep(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateTutorialStepCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetTutorialShouldReturnProperResult(bool isError)
    {
        var response = new TutorialResponse();

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetTutorialQuery>()))
                .ReturnsAsync(Result<TutorialResponse>.Failure(NotFoundError));

            var result = await _sut.GetTutorial(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetTutorialQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetTutorialQuery>()))
                .ReturnsAsync(Result<TutorialResponse>.Success(response));

            var result = await _sut.GetTutorial(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetTutorialQuery>()));
            result.Should().BeOfType<ActionResult<TutorialResponse>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetTutorialsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<TutorialResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetTutorialsQuery>()))
                .ReturnsAsync(Result<ListResponse<TutorialResponse>>.Failure(NotFoundError));

            var result = await _sut.GetTutorials();

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetTutorialsQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetTutorialsQuery>()))
                .ReturnsAsync(Result<ListResponse<TutorialResponse>>.Success(response));

            var result = await _sut.GetTutorials();

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetTutorialsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<TutorialResponse>?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task SearchMarketplaceItemsShouldReturnProperResult(bool isError)
    {
        var response = new PagedResponse<ListResponse<TutorialResponse>>()
        {
            Data = new ListResponse<TutorialResponse>()
            {
                Count = 0,
                Items = []
            }
        };
        
        var request = new SearchTutorialsFilters()
        {
            PageNumber = 1,
            PageSize = 10
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<SearchTutorialsQuery>()))
                .ReturnsAsync(Result<PagedResponse<ListResponse<TutorialResponse>>>.Failure(BadRequestError));

            var result = await _sut.SearchTutorials(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<SearchTutorialsQuery>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<SearchTutorialsQuery>()))
                .ReturnsAsync(Result<PagedResponse<ListResponse<TutorialResponse>>>.Success(response));

            var result = await _sut.SearchTutorials(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<SearchTutorialsQuery>()));
            result.Should().BeOfType<ActionResult<PagedResponse<ListResponse<TutorialResponse>>?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task AssignTutorialToUserShouldReturnProperResult(bool isError)
    {
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<AssignTutorialToUserCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.AssignTutorialToUser(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<AssignTutorialToUserCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<AssignTutorialToUserCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.AssignTutorialToUser(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<AssignTutorialToUserCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
}