using HobbyHub.Application.Infrastructure.RequestHandling;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HobbyHub.WebApi.Controllers;

/// <summary>
/// Marketplace Controller
/// </summary>
[Authorize]
public class MarketplaceController(IHobbyHubRequestHandler requestHandler) : ControllerBase
{
    /// <summary>
    /// Creates a new marketplace item.
    /// </summary>
    /// <remarks>
    ///     "price" property has to be like 5,4 and not like 5.4
    /// </remarks>
    /// <param name="request"></param>
    [HttpPost]
    [Route("marketplaceItem")]
    public async Task<ActionResult<CommandResponse>> CreateMarketplaceItem([FromForm] CreateMarketplaceItemRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreateMarketplaceItemCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Deletes saved marketplace item.
    /// </summary>
    /// <param name="request"></param>
    [HttpDelete]
    [Route("savedMarketplaceItem")]
    public async Task<ActionResult<CommandResponse>> DeleteSavedMarketplaceItem([FromBody] DeleteSavedMarketplaceItemRequest request)
    {
        var result = await requestHandler.HandleRequest(new DeleteSavedMarketplaceItemCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Deletes marketplace item.
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete]
    [Route("marketplaceItem/{id:int}")]
    public async Task<ActionResult<CommandResponse>> DeleteMarketplaceItem(int id)
    {
        var result = await requestHandler.HandleRequest(new DeleteMarketplaceItemCommand(id));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Adds marketplace item to user's saved marketplace items.
    /// </summary>
    /// <param name="request"></param>
    [HttpPost]
    [Route("savedMarketplaceItem")]
    public async Task<ActionResult<CommandResponse>> AddSavedMarketplaceItem([FromBody] CreateSavedMarketplaceItemRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreateSavedMarketplaceItemCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Marks marketplace item as sold.
    /// </summary>
    /// <param name="marketplaceItemId"></param>
    /// <param name="userId"></param>
    [HttpPut]
    [Route("marketplaceItem/{marketplaceItemId:int}/mark-as-sold")]
    public async Task<ActionResult<CommandResponse>> MarkMarketplaceItemAsSold(int marketplaceItemId, [FromQuery] int userId)
    {
        var result = await requestHandler.HandleRequest(new BuyMarketplaceItemCommand(userId, marketplaceItemId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets all marketplace items.
    /// </summary>
    /// <param name="request"></param>
    [HttpGet]
    [Route("marketplaceItems")]
    public async Task<ActionResult<ListResponse<MarketplaceItemResponse>>> GetMarketplaceItems()
    {
        var result = await requestHandler.HandleRequest(new GetMarketplaceItemsQuery());
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Searches marketplace items.
    /// </summary>
    /// <param name="request"></param>
    [HttpPost]
    [Route("marketplaceItemsSearch")]
    public async Task<ActionResult<PagedResponse<ListResponse<MarketplaceItemResponse>>>> MarketplaceItemsSearch([FromBody] SearchMarketplaceItemsFilters request)
    {
        var result = await requestHandler.HandleRequest(new SearchMarketplaceItemsQuery(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
}