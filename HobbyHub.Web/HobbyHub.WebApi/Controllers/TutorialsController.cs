using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.RequestHandlers.Tutorials.AssignTutorialToUser;
using HobbyHub.Application.RequestHandlers.Tutorials.CreateTutorial;
using HobbyHub.Application.RequestHandlers.Tutorials.CreateTutorialStep;
using HobbyHub.Application.RequestHandlers.Tutorials.GetTutorial;
using HobbyHub.Application.RequestHandlers.Tutorials.GetTutorials;
using HobbyHub.Application.RequestHandlers.Tutorials.SearchTutorials;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Tutorials;
using HobbyHub.Contract.Responses.Tutorials;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HobbyHub.WebApi.Controllers;

/// <summary>
/// Tutorials Controller
/// </summary>
[Authorize]
public class TutorialsController(IHobbyHubRequestHandler requestHandler) : ControllerBase
{
    /// <summary>
    /// Creates a new tutorial.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="request"></param>
    [HttpPost]
    [Route("tutorial")]
    public async Task<ActionResult<CommandResponse>> CreateTutorial([FromForm] CreateTutorialRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreateTutorialCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Creates a new tutorial step.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="request"></param>
    [HttpPost]
    [Route("tutorialStep")]
    public async Task<ActionResult<CommandResponse>> CreateTutorialStep([FromForm] CreateTutorialStepRequest request)
    {
        var result = await requestHandler.HandleRequest(new CreateTutorialStepCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets tutorial.
    /// </summary>
    /// <param name="tutorialId"></param>
    [HttpGet]
    [Route("tutorial/{tutorialId:int}")]
    public async Task<ActionResult<TutorialResponse>> GetTutorial(int tutorialId)
    {
        var result = await requestHandler.HandleRequest(new GetTutorialQuery(tutorialId));
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Gets all tutorials.
    /// </summary>
    [HttpGet]
    [Route("/tutorials")]
    public async Task<ActionResult<ListResponse<TutorialResponse>>> GetTutorials()
    {
        var result = await requestHandler.HandleRequest(new GetTutorialsQuery());
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Searches tutorials by filters.
    /// </summary>
    [HttpPost]
    [Route("/tutorialsSearch")]
    public async Task<ActionResult<PagedResponse<ListResponse<TutorialResponse>>>> SearchTutorials([FromBody] SearchTutorialsFilters request)
    {
        var result = await requestHandler.HandleRequest(new SearchTutorialsQuery(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Assigns tutorial to user (gives user access to requested tutorial).
    /// </summary>
    [HttpPut]
    [Route("/tutorial/{tutorialId:int}/user/{userId:int}")]
    public async Task<ActionResult<CommandResponse>> AssignTutorialToUser(int userId, int tutorialId)
    {
        var result = await requestHandler.HandleRequest(new AssignTutorialToUserCommand(userId, tutorialId));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
}