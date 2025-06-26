using FluentAssertions;
using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Events.GetUserEvents;
using HobbyHub.Application.RequestHandlers.Marketplace.GetSavedMarketplaceItems;
using HobbyHub.Application.RequestHandlers.Marketplace.GetUserCreatedMarketplaceItems;
using HobbyHub.Application.RequestHandlers.Marketplace.GetUserOrderedMarketplaceItems;
using HobbyHub.Application.RequestHandlers.Tutorials.GetUserAssignedTutorials;
using HobbyHub.Application.RequestHandlers.Tutorials.GetUserTutorials;
using HobbyHub.Application.RequestHandlers.Users.FollowUser;
using HobbyHub.Application.RequestHandlers.Users.GetUserFollows;
using HobbyHub.Application.RequestHandlers.Users.GetUserSummary;
using HobbyHub.Application.RequestHandlers.Users.SearchUsers;
using HobbyHub.Application.RequestHandlers.Users.UnfollowUser;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Users;
using HobbyHub.Contract.Responses.Events;
using HobbyHub.Contract.Responses.Marketplace;
using HobbyHub.Contract.Responses.Tutorials;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace HobbyHub.WebApi.UnitTests.Controllers;

public class UsersControllerTests
{
    private Mock<IHobbyHubRequestHandler> _requestHandler;
    private UsersController _sut;

    private static readonly Error NotFoundError = new Error("test", "test", StatusCodes.Status404NotFound);
    private static readonly Error BadRequestError = new Error("test", "test", StatusCodes.Status400BadRequest);

    [SetUp]
    public void SetUp()
    {
        _requestHandler = new Mock<IHobbyHubRequestHandler>();
        _sut = new UsersController(_requestHandler.Object);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task FollowUserShouldReturnProperResult(bool isError)
    {
        var request = new FollowUserRequest()
        {
            UserId = 1,
            UserToFollowId = 2
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<FollowUserCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.Follow(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<FollowUserCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<FollowUserCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.Follow(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<FollowUserCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task UnFollowUserShouldReturnProperResult(bool isError)
    {
        var request = new UnfollowUserRequest()
        {
            UserId = 1,
            UserToUnfollowId = 2
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<UnfollowUserCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.Unfollow(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<UnfollowUserCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<UnfollowUserCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.Unfollow(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<UnfollowUserCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetFollowsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<UserSummaryResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserFollowsQuery>()))
                .ReturnsAsync(Result<ListResponse<UserSummaryResponse>>.Failure(NotFoundError));

            var result = await _sut.Follows(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserFollowsQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserFollowsQuery>()))
                .ReturnsAsync(Result<ListResponse<UserSummaryResponse>>.Success(response));

            var result = await _sut.Follows(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserFollowsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<UserSummaryResponse>?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task UsersSearchShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<UserSummaryResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<SearchUsersQuery>()))
                .ReturnsAsync(Result<ListResponse<UserSummaryResponse>>.Failure(NotFoundError));

            var result = await _sut.UsersSearch("test");

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<SearchUsersQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<SearchUsersQuery>()))
                .ReturnsAsync(Result<ListResponse<UserSummaryResponse>>.Success(response));

            var result = await _sut.UsersSearch("keyword");

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<SearchUsersQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<UserSummaryResponse>?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetUserEventsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<EventResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserEventsQuery>()))
                .ReturnsAsync(Result<ListResponse<EventResponse>>.Failure(NotFoundError));

            var result = await _sut.UserEvents(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserEventsQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserEventsQuery>()))
                .ReturnsAsync(Result<ListResponse<EventResponse>>.Success(response));

            var result = await _sut.UserEvents(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserEventsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<EventResponse>?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetUserSavedMarketplaceItemsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<MarketplaceItemResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetSavedMarketplaceItemsQuery>()))
                .ReturnsAsync(Result<ListResponse<MarketplaceItemResponse>>.Failure(NotFoundError));

            var result = await _sut.SavedMarketplaceItems(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetSavedMarketplaceItemsQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetSavedMarketplaceItemsQuery>()))
                .ReturnsAsync(Result<ListResponse<MarketplaceItemResponse>>.Success(response));

            var result = await _sut.SavedMarketplaceItems(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetSavedMarketplaceItemsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<MarketplaceItemResponse>?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetUserCreatedMarketplaceItemsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<MarketplaceItemResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserCreatedMarketplaceItemsQuery>()))
                .ReturnsAsync(Result<ListResponse<MarketplaceItemResponse>>.Failure(NotFoundError));

            var result = await _sut.CreatedMarketplaceItems(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserCreatedMarketplaceItemsQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserCreatedMarketplaceItemsQuery>()))
                .ReturnsAsync(Result<ListResponse<MarketplaceItemResponse>>.Success(response));

            var result = await _sut.CreatedMarketplaceItems(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserCreatedMarketplaceItemsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<MarketplaceItemResponse>?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetUserOrderedMarketplaceItemsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<MarketplaceItemResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserOrderedMarketplaceItemsQuery>()))
                .ReturnsAsync(Result<ListResponse<MarketplaceItemResponse>>.Failure(NotFoundError));

            var result = await _sut.OrderedMarketplaceItems(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserOrderedMarketplaceItemsQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserOrderedMarketplaceItemsQuery>()))
                .ReturnsAsync(Result<ListResponse<MarketplaceItemResponse>>.Success(response));

            var result = await _sut.OrderedMarketplaceItems(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserOrderedMarketplaceItemsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<MarketplaceItemResponse>?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetUserSummaryShouldReturnProperResult(bool isError)
    {
        var response = new UserSummaryResponse()
        {
            Name = "name",
            Surname = "surname"
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserSummaryQuery>()))
                .ReturnsAsync(Result<UserSummaryResponse>.Failure(NotFoundError));

            var result = await _sut.UserSummary(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserSummaryQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserSummaryQuery>()))
                .ReturnsAsync(Result<UserSummaryResponse>.Success(response));

            var result = await _sut.UserSummary(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserSummaryQuery>()));
            result.Should().BeOfType<ActionResult<UserSummaryResponse>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetUserCreatedTutorialsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<TutorialResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserTutorialsQuery>()))
                .ReturnsAsync(Result<ListResponse<TutorialResponse>>.Failure(NotFoundError));

            var result = await _sut.UserTutorials(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserTutorialsQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserTutorialsQuery>()))
                .ReturnsAsync(Result<ListResponse<TutorialResponse>>.Success(response));

            var result = await _sut.UserTutorials(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserTutorialsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<TutorialResponse>?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetUserAssignedTutorialsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<TutorialResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserAssignedTutorialsQuery>()))
                .ReturnsAsync(Result<ListResponse<TutorialResponse>>.Failure(NotFoundError));

            var result = await _sut.UserAssignedTutorials(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserAssignedTutorialsQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetUserAssignedTutorialsQuery>()))
                .ReturnsAsync(Result<ListResponse<TutorialResponse>>.Success(response));

            var result = await _sut.UserAssignedTutorials(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetUserAssignedTutorialsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<TutorialResponse>?>>();
        }
    }
}