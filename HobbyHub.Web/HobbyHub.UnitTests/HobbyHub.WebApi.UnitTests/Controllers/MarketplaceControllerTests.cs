using FluentAssertions;
using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Application.RequestHandlers.Marketplace.BuyMarketplaceItem;
using HobbyHub.Application.RequestHandlers.Marketplace.CreateMarketplaceItem;
using HobbyHub.Application.RequestHandlers.Marketplace.CreateSavedMarketplaceItem;
using HobbyHub.Application.RequestHandlers.Marketplace.DeleteMarketplaceItem;
using HobbyHub.Application.RequestHandlers.Marketplace.DeleteSavedMarketplaceItem;
using HobbyHub.Application.RequestHandlers.Marketplace.GetMarketplaceItems;
using HobbyHub.Application.RequestHandlers.Marketplace.SearchMarketplaceItems;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Marketplace;
using HobbyHub.Contract.Responses.Marketplace;
using HobbyHub.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace HobbyHub.WebApi.UnitTests.Controllers;

public class MarketplaceControllerTests
{
    private Mock<IHobbyHubRequestHandler> _requestHandler;
    private MarketplaceController _sut;

    private static readonly Error NotFoundError = new Error("test", "test", StatusCodes.Status404NotFound);
    private static readonly Error BadRequestError = new Error("test", "test", StatusCodes.Status400BadRequest);

    [SetUp]
    public void SetUp()
    {
        _requestHandler = new Mock<IHobbyHubRequestHandler>();
        _sut = new MarketplaceController(_requestHandler.Object);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task CreateMarketplaceItemShouldReturnProperResult(bool isError)
    {
        var request = new CreateMarketplaceItemRequest()
        {
            Title = "title",
            Type = "type",
            Category = "category"
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateMarketplaceItemCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.CreateMarketplaceItem(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateMarketplaceItemCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateMarketplaceItemCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.CreateMarketplaceItem(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateMarketplaceItemCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task DeleteSavedMarketplaceItemShouldReturnProperResult(bool isError)
    {
        var request = new DeleteSavedMarketplaceItemRequest()
        {
            UserId = 1,
            MarketplaceItemId = 1
        };

        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteSavedMarketplaceItemCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.DeleteSavedMarketplaceItem(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteSavedMarketplaceItemCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteSavedMarketplaceItemCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.DeleteSavedMarketplaceItem(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteSavedMarketplaceItemCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task DeleteMarketplaceItemShouldReturnProperResult(bool isError)
    {
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteMarketplaceItemCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.DeleteMarketplaceItem(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteMarketplaceItemCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<DeleteMarketplaceItemCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.DeleteMarketplaceItem(1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<DeleteMarketplaceItemCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task AddSavedMarketplaceItemShouldReturnProperResult(bool isError)
    {
        var request = new CreateSavedMarketplaceItemRequest()
        {
            UserId = 1,
            MarketplaceItemId = 1
        };
        
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateSavedMarketplaceItemCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.AddSavedMarketplaceItem(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateSavedMarketplaceItemCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<CreateSavedMarketplaceItemCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.AddSavedMarketplaceItem(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<CreateSavedMarketplaceItemCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task MarkMarketplaceItemAsSoldShouldReturnProperResult(bool isError)
    {
        var response = new CommandResponse(1);

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<BuyMarketplaceItemCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Failure(BadRequestError));

            var result = await _sut.MarkMarketplaceItemAsSold(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<BuyMarketplaceItemCommand>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<BuyMarketplaceItemCommand>()))
                .ReturnsAsync(Result<CommandResponse>.Success(response));

            var result = await _sut.MarkMarketplaceItemAsSold(1, 1);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<BuyMarketplaceItemCommand>()));
            result.Should().BeOfType<ActionResult<CommandResponse?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetAllMarketplaceItemsShouldReturnProperResult(bool isError)
    {
        var response = new ListResponse<MarketplaceItemResponse>()
        {
            Count = 0,
            Items = []
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetMarketplaceItemsQuery>()))
                .ReturnsAsync(Result<ListResponse<MarketplaceItemResponse>>.Failure(NotFoundError));

            var result = await _sut.GetMarketplaceItems();

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetMarketplaceItemsQuery>()));
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<GetMarketplaceItemsQuery>()))
                .ReturnsAsync(Result<ListResponse<MarketplaceItemResponse>>.Success(response));

            var result = await _sut.GetMarketplaceItems();

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<GetMarketplaceItemsQuery>()));
            result.Should().BeOfType<ActionResult<ListResponse<MarketplaceItemResponse>?>>();
        }
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task SearchMarketplaceItemsShouldReturnProperResult(bool isError)
    {
        var response = new PagedResponse<ListResponse<MarketplaceItemResponse>>()
        {
            Data = new ListResponse<MarketplaceItemResponse>()
            {
                Count = 0,
                Items = []
            }
        };
        
        var request = new SearchMarketplaceItemsFilters()
        {
            PageNumber = 1,
            PageSize = 10
        };

        if (isError)
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<SearchMarketplaceItemsQuery>()))
                .ReturnsAsync(Result<PagedResponse<ListResponse<MarketplaceItemResponse>>>.Failure(BadRequestError));

            var result = await _sut.MarketplaceItemsSearch(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<SearchMarketplaceItemsQuery>()));
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
        else
        {
            _requestHandler.Setup(r => r.HandleRequest(It.IsAny<SearchMarketplaceItemsQuery>()))
                .ReturnsAsync(Result<PagedResponse<ListResponse<MarketplaceItemResponse>>>.Success(response));

            var result = await _sut.MarketplaceItemsSearch(request);

            _requestHandler.Verify(r => r.HandleRequest(It.IsAny<SearchMarketplaceItemsQuery>()));
            result.Should().BeOfType<ActionResult<PagedResponse<ListResponse<MarketplaceItemResponse>>?>>();
        }
    }
}