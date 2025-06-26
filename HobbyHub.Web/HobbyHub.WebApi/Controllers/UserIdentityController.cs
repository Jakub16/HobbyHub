using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.RequestHandlers.UserIdentity.LoginUser;
using HobbyHub.Application.RequestHandlers.UserIdentity.RegisterUser;
using HobbyHub.Application.RequestHandlers.UserIdentity.UserAuthenticationStatus;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.UserIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HobbyHub.WebApi.Controllers;

/// <summary>
/// UserIdentityController
/// </summary>
[Route("[controller]")]
public class UserIdentityController(IHobbyHubRequestHandler requestHandler) : ControllerBase
{
    /// <summary>
    /// Registers user.
    /// </summary>
    /// <param name="request"></param>
    [HttpPost, Route("register")]
    [AllowAnonymous]
    public async Task<ActionResult<CommandResponse>> Register([FromBody] RegisterUserRequest request)
    {
        var result = await requestHandler.HandleRequest(new RegisterUserCommand(request));

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Logins user.
    /// </summary>
    /// <param name="request"></param>
    [HttpPost, Route("login")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Login([FromBody] LoginUserRequest request)
    {
        var result = await requestHandler.HandleRequest(new LoginUserCommand(request));
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return result.Outcome;
    }
    
    /// <summary>
    /// Verifies if provided token is valid.
    /// </summary>
    /// <param name="token"></param>
    [HttpPost, Route("verify")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> Verify(string token)
    {
        var result = await requestHandler.HandleRequest(new GetUserAuthenticationStatusQuery(token));

        return result;
    }
}