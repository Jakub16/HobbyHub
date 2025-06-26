using HobbyHub.UserIdentity.Provider.Abstractions;
using MediatR;

namespace HobbyHub.Application.RequestHandlers.UserIdentity.UserAuthenticationStatus;

public class GetUserAuthenticationStatusQueryHandler(IJwtService jwtService) 
    : IRequestHandler<GetUserAuthenticationStatusQuery, bool>
{
    public Task<bool> Handle(GetUserAuthenticationStatusQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(jwtService.Verify(request.Token));
    }
}