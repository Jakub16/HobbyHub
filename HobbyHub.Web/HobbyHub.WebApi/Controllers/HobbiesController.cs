using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.CreateFavoriteHobby;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.DeleteFavoriteHobby;
using HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.GetFavoriteHobbies;
using HobbyHub.Application.RequestHandlers.Hobbies.PredefinedHobbies.CreatePredefinedHobby;
using HobbyHub.Application.RequestHandlers.Hobbies.PredefinedHobbies.GetPredefinedHobbies;
using HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.CreateUserHobby;
using HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.DeleteUserHobby;
using HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.GetUserHobbies;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Hobbies;
using HobbyHub.Contract.Responses.Hobbies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace HobbyHub.WebApi.Controllers;

/// <summary>
/// Hobbies Controller
/// </summary>
[Authorize]
public class HobbiesController(IHobbyHubRequestHandler requestHandler) : ControllerBase
{
    /// <summary>
    /// Creates predefined hobby.
    /// </summary>
    /// <param name="request"></param>
    [HttpPost, Route("predefinedHobby")]
    [SwaggerResponse(200, typeof(CommandResponse))]
    public async Task<ActionResult<CommandResponse>> CreatePredefinedHobby([FromBody] CreateHobbyRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreatePredefinedHobbyCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Creates user (custom) hobby.
    /// </summary>
    /// <param name="request"></param>
    [HttpPost, Route("userHobby")]
    [SwaggerResponse(200, typeof(CommandResponse))]
    public async Task<ActionResult<CommandResponse>> CreateUserHobby([FromBody] CreateUserHobbyRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreateUserHobbyCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Adds existing custom or predefined hobby to user's favorites.
    /// </summary>
    /// <remarks>
    ///     Set "IsHobbyPredefined" to false if hobbyId references to custom hobby created by a user
    /// </remarks>
    /// <param name="request"></param>
    [HttpPost, Route("favoriteHobby")]
    [SwaggerResponse(200, typeof(CommandResponse))]
    public async Task<ActionResult<CommandResponse>> CreateFavoriteHobby([FromBody] CreateFavoriteHobbyRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreateFavoriteHobbyCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets all predefined hobbies.
    /// </summary>
    [HttpGet, Route("predefinedHobbies")]
    [SwaggerResponse(200, typeof(ListResponse<HobbyResponse>))]
    public async Task<ActionResult<ListResponse<HobbyResponse>>> GetPredefinedHobbies()
    {
        var result = await requestHandler.HandleRequest(new GetPredefinedHobbiesQuery());
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Deletes favorite hobby with the provided id.
    /// </summary>
    /// <param name="favoriteHobbyId"></param>
    [HttpDelete, Route("favoriteHobby/{favoriteHobbyId:int}")]
    [SwaggerResponse(200, typeof(ListResponse<HobbyResponse>))]
    public async Task<ActionResult<CommandResponse>> DeleteFavoriteHobby(int favoriteHobbyId)
    {
        var result = await requestHandler.HandleRequest(new DeleteFavoriteHobbyCommand(favoriteHobbyId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
}