using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.RequestHandlers.Activities.DeleteActivity;
using HobbyHub.Application.RequestHandlers.Activities.EndActivity;
using HobbyHub.Application.RequestHandlers.Activities.GetUserActiveActivity;
using HobbyHub.Application.RequestHandlers.Activities.StartActivity;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Activities;
using HobbyHub.Contract.Responses.Activities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace HobbyHub.WebApi.Controllers;

/// <summary>
/// Activities Controller
/// </summary>
[Produces("application/json")]
[Authorize]
public class ActivitiesController(IHobbyHubRequestHandler requestHandler) : ControllerBase
{
    /// <summary>
    /// Starts activity.
    /// </summary>
    /// <param name="startActivityRequest"></param>
    /// <remarks>
    ///     Set "IsHobbyPredefined" to false if hobbyId references to custom hobby created by a user
    /// </remarks>
    [SwaggerResponse(200, typeof(ActionResult<CommandResponse?>))]
    [HttpPost, Route("activity")]
    public async Task<ActionResult<CommandResponse?>> StartActivity([FromBody] StartActivityRequest startActivityRequest)
    {
        var result = await requestHandler.HandleRequest(new StartActivityCommand(startActivityRequest));
        
        if (!result.IsSuccess && result.Error?.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    /// <summary>
    /// Ends activity.
    /// </summary>
    /// <remarks>
    ///     Set "IsDistanceAvailable" to true if selected hobby allows for distance tracking
    ///     "Distance" property has to be like 5,4 and not like 5.4
    /// </remarks>
    /// <param name="request"></param>
    [HttpPut, Route("activity")]
    [SwaggerResponse(200, typeof(ActionResult<CommandResponse?>))]
    public async Task<ActionResult<CommandResponse?>> EndActivity([FromForm] EndActivityRequest request)
    {
        var result = await requestHandler.HandleRequest(new EndActivityCommand(request));
        
        if (!result.IsSuccess && result.Error?.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Deletes activity with the provided id.
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete, Route("activity/{id:int}")]
    [SwaggerResponse(200, typeof(ActionResult<CommandResponse>))]
    public async Task<ActionResult<CommandResponse>> DeleteActivity(int id)
    {
        var result = await requestHandler.HandleRequest(new DeleteActivityCommand(id));
        
        if (!result.IsSuccess && result.Error?.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
}